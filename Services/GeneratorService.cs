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
            // ���� Roslyn �﷨��
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetCompilationUnitRoot();

            // ��ȡ���� using ָ��
            var usings = root.Usings.Select(u => u.ToString()).ToList();
            var usingText = string.Join("", usings);

            // ���������ռ�����
            var namespaceDeclaration = root.DescendantNodes()
                                           .OfType<NamespaceDeclarationSyntax>()
                                           .FirstOrDefault()?.Name.ToString() ??
                                           root.DescendantNodes()
                                           .OfType<FileScopedNamespaceDeclarationSyntax>()
                                           .FirstOrDefault()?.Name.ToString() ??
                                           throw new Exception("�����ռ�δ�ҵ�");

            // ����������
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            List<EntityInfo> entityInfos = new List<EntityInfo>();
            foreach (var classDeclaration in classDeclarations)
            {
                var entityInfo = new EntityInfo();
                entityInfo.Using = usingText;
                entityInfo.Namespace = namespaceDeclaration;// ��ȡ�����ռ�
                // ����������
                await ParseClassInfoAsync(classDeclaration, entityInfo);
                entityInfos.Add(entityInfo);
            }

            return entityInfos;
        }

        private async Task ParseClassInfoAsync(ClassDeclarationSyntax classDeclaration, EntityInfo entityInfo)
        {
            // ����
            entityInfo.ClassName = classDeclaration.Identifier.Text;
            // ��ȡ���ע��
            entityInfo.Description = GetLeadingComments(classDeclaration);
            // ��ȡ��Ĵ���
            entityInfo.OriginalCode = classDeclaration.ToString();

            // ��ȡ������
            entityInfo.Attributes = GetClassAttributes(classDeclaration);


            // �������е�����
            foreach (var property in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
            {
                var propertyName = property.Identifier.Text;
                var propertyType = property.Type.ToString();
                // ��ȡ���Ե�ע��
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
                        length = 100; // Ĭ�ϳ���Ϊ100
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
                            length = -1; // ���û���ṩ����ĳ��ȣ�����Ϊ-1��ʾʹ�����ݿ�Ĭ��ֵ���������ֵ
                        }
                    }
                }
                else if (propertyType.Contains("decimal"))
                {
                    length = 18; // ����decimal���ͣ�Ĭ�ϳ���Ϊ18
                }

                var defaultValue = property.Initializer?.Value.ToString();


                // ���� CodeColumnsViewModel
                var column = new PropertyInfo
                {
                    Name = propertyName,
                    Description = propertyComment,
                    Type = propertyType,
                    DefaultValue = defaultValue,
                    Required = !propertyType.Contains("?"), // ���ж��Ƿ�Ϊ����
                    DecimalDigits = propertyType.Contains("decimal") ? 4 : null, // ���ж�С��λ�����˴���Ϊʾ���߼�
                    MaxLength = length, // �������MaxLength��������ȡֵ������Ĭ�ϳ���Ϊ100�������MaxLength��Ϊ-1
                    //����ж��Ƿ�Ϊ���ݿ��ֶ�?
                    IsReference = await IsReferenceAsync(property),
                    Attributes = GetPropertyAttributes(property),
                };

                entityInfo.Properties.Add(column);
            }
        }

        private List<EntityAttribute> GetClassAttributes(ClassDeclarationSyntax classDeclaration)
        {
            var attributes = new List<EntityAttribute>();

            // ������� AttributeLists
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    // ��ȡ��������
                    var attributeName = attribute.Name.ToString();

                    // ��ȡ���Բ���
                    var arguments = attribute.ArgumentList?.Arguments
                        .Select(arg => arg.ToString())
                        .ToList();

                    // ���� EntityAttribute ʵ��
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

            // ������� AttributeLists
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    // ��ȡ��������
                    var attributeName = attribute.Name.ToString();

                    // ��ȡ���Բ���
                    var arguments = attribute.ArgumentList?.Arguments
                        .Select(arg => arg.ToString())
                        .ToList();

                    // ���� EntityAttribute ʵ��
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
            // ��ȡ�ڵ�ǰ��ע��
            var trivia = node.GetLeadingTrivia()
                             .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                             .Select(t => t.ToString().Trim().Replace("<summary>", "").Replace("</summary>", "").Replace("///", "").Trim().Replace("\r\n", "").Replace("\n", ""));

            return string.Join(Environment.NewLine, trivia);
        }

        #region �ж��Ƿ���������
        private async Task<bool> IsReferenceAsync(PropertyDeclarationSyntax property)
        {
            // ��ȡ��������
            var propertyType = property.Type.ToString();

            // ����Ƿ�Ϊ�������ͣ��� List<T> �� IEnumerable<T>��
            if (propertyType.StartsWith("List<") || propertyType.StartsWith("IEnumerable<") || propertyType.StartsWith("ICollection<"))
            {
                return true; // ��������ͨ������������
            }

            // ����Ƿ�Ϊ�������ͣ��ų� string ���ͣ�
            if (!IsPrimitiveOrString(propertyType) && !await IsEnumAsync(propertyType))
            {
                return true; // �������ͣ��� string��ͨ�����������ݿ��ֶ�
            }

            return false;
        }

        private bool IsPrimitiveOrString(string typeName)
        {
            // �ж��Ƿ�Ϊ�����������ͻ� string
            var primitiveTypes = new HashSet<string>
            {
                "int", "long", "short", "byte", "bool", "float", "double", "decimal", "string", "char", "DateTime", "Guid", "TimeSpan", "byte[]",
                "int?", "long?", "short?", "byte?", "bool?", "float?", "double?", "decimal?", "string?", "char?", "DateTime?", "Guid?", "TimeSpan?", "byte[]?",
            };

            return primitiveTypes.Contains(typeName);
        }

        private async Task<bool> IsEnumAsync(string typeName)
        {
            // �ж��Ƿ�Ϊö������
            // �˴����Ը��ݾ��������Ľ�һ�����ƣ������������Ƿ��ڵ�ǰ��Ŀ�ж���Ϊö��
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
