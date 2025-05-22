using CodeFirstGenerator.Controllers;
using CodeFirstGenerator.Data;
using CodeFirstGenerator.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstGenerator.Services
{
    public class GeneratorService : IGeneratorService
    {
        private Configuration? configuration;
        private readonly ApplicationDbContext db;

        public GeneratorService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<List<EntityInfo>> ParseClassCodeAsync(string code)
        {
            // 创建 Roslyn 语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetCompilationUnitRoot();

            // 获取所有 using 指令
            var usings = root.Usings.Select(u => u.ToString()).ToList();
            var usingText = string.Join("", usings);

            // 查找命名空间声明
            var namespaceDeclaration = root.DescendantNodes()
                                           .OfType<NamespaceDeclarationSyntax>()
                                           .FirstOrDefault()?.Name.ToString() ??
                                           root.DescendantNodes()
                                           .OfType<FileScopedNamespaceDeclarationSyntax>()
                                           .FirstOrDefault()?.Name.ToString() ??
                                           throw new Exception("命名空间未找到");

            // 查找类声明
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            List<EntityInfo> entityInfos = new List<EntityInfo>();
            foreach (var classDeclaration in classDeclarations)
            {
                var entityInfo = new EntityInfo();
                entityInfo.Using = usingText;
                entityInfo.Namespace = namespaceDeclaration;// 获取命名空间
                // 分析类声明
                await ParseClassInfoAsync(classDeclaration, entityInfo);
                entityInfos.Add(entityInfo);
            }

            return entityInfos;
        }

        private async Task ParseClassInfoAsync(ClassDeclarationSyntax classDeclaration, EntityInfo entityInfo)
        {
            // 类名
            entityInfo.ClassName = classDeclaration.Identifier.Text;
            // 获取类的注释
            entityInfo.Description = GetLeadingComments(classDeclaration);
            // 获取类的代码
            entityInfo.OriginalCode = classDeclaration.ToString();

            // 提取类特性
            entityInfo.Attributes = GetClassAttributes(classDeclaration);


            // 遍历类中的属性
            foreach (var property in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
            {
                var propertyName = property.Identifier.Text;
                var propertyType = property.Type.ToString();
                // 获取属性的注释
                var propertyComment = GetLeadingComments(property);

                int? length = null;
                if (propertyType.Contains("string"))
                {
                    var maxLengthAttribute = property.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Where(attr => attr.Name.ToString().Contains("MaxLength"))
                        .FirstOrDefault();
                    if (maxLengthAttribute == null)
                    {
                        length = 100; // 默认长度为100
                    }
                    else
                    {
                        var lengthStr = maxLengthAttribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression.ToString();
                        if (int.TryParse(lengthStr, out var lengthValue))
                        {
                            length = lengthValue;
                        }
                        else
                        {
                            length = -1; // 如果没有提供具体的长度，则标记为-1表示使用数据库默认值或最大允许值
                        }
                    }
                }
                else if (propertyType.Contains("decimal"))
                {
                    length = 18; // 对于decimal类型，默认长度为18
                }

                var defaultValue = property.Initializer?.Value.ToString();


                // 创建 CodeColumnsViewModel
                var column = new PropertyInfo
                {
                    Name = propertyName,
                    Description = propertyComment,
                    Type = propertyType,
                    DefaultValue = defaultValue,
                    Required = !propertyType.Contains("?"), // 简单判断是否为必填
                    DecimalDigits = propertyType.Contains("decimal") ? 4 : null, // 简单判断小数位数，此处仅为示例逻辑
                    MaxLength = length, // 如果存在MaxLength特性则提取值，否则默认长度为100，如果是MaxLength则为-1
                    //如何判断是否为数据库字段?
                    IsReference = await IsReferenceAsync(property),
                    Attributes = GetPropertyAttributes(property),
                };

                entityInfo.Properties.Add(column);
            }
        }

        private List<EntityAttribute> GetClassAttributes(ClassDeclarationSyntax classDeclaration)
        {
            var attributes = new List<EntityAttribute>();

            // 遍历类的 AttributeLists
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    // 提取特性名称
                    var attributeName = attribute.Name.ToString();

                    // 提取特性参数
                    var arguments = attribute.ArgumentList?.Arguments
                        .Select(arg => arg.ToString())
                        .ToList();

                    // 创建 EntityAttribute 实例
                    var entityAttribute = new EntityAttribute
                    {
                        Name = attributeName,
                        Arguments = arguments != null ? string.Join(", ", arguments) : string.Empty
                    };

                    attributes.Add(entityAttribute);
                }
            }

            return attributes;
        }

        private List<PropertyAttribute> GetPropertyAttributes(PropertyDeclarationSyntax classDeclaration)
        {
            var attributes = new List<PropertyAttribute>();

            // 遍历类的 AttributeLists
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    // 提取特性名称
                    var attributeName = attribute.Name.ToString();

                    // 提取特性参数
                    var arguments = attribute.ArgumentList?.Arguments
                        .Select(arg => arg.ToString())
                        .ToList();

                    // 创建 EntityAttribute 实例
                    var entityAttribute = new PropertyAttribute
                    {
                        Name = attributeName,
                        Arguments = arguments != null ? string.Join(", ", arguments) : string.Empty
                    };

                    attributes.Add(entityAttribute);
                }
            }

            return attributes;
        }

        private string GetLeadingComments(SyntaxNode node)
        {
            // 获取节点前的注释
            var trivia = node.GetLeadingTrivia()
                             .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                             .Select(t => t.ToString().Trim().Replace("<summary>", "").Replace("</summary>", "").Replace("///", "").Trim().Replace("\r\n", "").Replace("\n", ""));

            return string.Join(Environment.NewLine, trivia);
        }

        #region 判断是否引用属性
        private async Task<bool> IsReferenceAsync(PropertyDeclarationSyntax property)
        {
            // 获取属性类型
            var propertyType = property.Type.ToString();

            // 检查是否为集合类型（如 List<T> 或 IEnumerable<T>）
            if (propertyType.StartsWith("List<") || propertyType.StartsWith("IEnumerable<") || propertyType.StartsWith("ICollection<"))
            {
                return true; // 集合类型通常是引用属性
            }

            // 检查是否为引用类型（排除 string 类型）
            if (!IsPrimitiveOrString(propertyType) && !await IsEnumAsync(propertyType))
            {
                return true; // 引用类型（非 string）通常不属于数据库字段
            }

            return false;
        }

        private bool IsPrimitiveOrString(string typeName)
        {
            // 判断是否为基础数据类型或 string
            var primitiveTypes = new HashSet<string>
            {
                "int", "long", "short", "byte", "bool", "float", "double", "decimal", "string", "char", "DateTime", "Guid", "TimeSpan", "byte[]",
                "int?", "long?", "short?", "byte?", "bool?", "float?", "double?", "decimal?", "string?", "char?", "DateTime?", "Guid?", "TimeSpan?", "byte[]?",
            };

            return primitiveTypes.Contains(typeName);
        }

        private async Task<bool> IsEnumAsync(string typeName)
        {
            // 判断是否为枚举类型
            // 此处可以根据具体上下文进一步完善，例如检查类型是否在当前项目中定义为枚举
            var enumTypes = (await GetConfigurationAsync()).EnumTypes ?? "";
            return enumTypes.Contains(typeName);
        }

        private async Task<Configuration> GetConfigurationAsync()
        {
            if (configuration != null) return configuration;

            configuration = await db.Configurations.FirstOrDefaultAsync();
            return configuration ?? new Configuration();
        }
        #endregion
    }
}
