# CodeFirstGenerator 代码生成器

### 说明
此README由AI自动生成，仅供参考。

## 项目概述
`CodeFirstGenerator` 是一个基于 .NET 9.0 开发的自用代码优先（Code First）代码生成器。该工具专注于依据预定义的实体类，高效生成特定类型的代码，以减少开发过程中手动编写重复代码的工作量，提升开发效率。

## 功能特性
### 实体类处理
- **接口定义**：定义了 `IEntity` 接口，为实体类统一提供 `Id` 属性，保证实体类的规范性。
- **信息获取**：能够通过 `EntityInfoGetListInput` 等数据传输对象获取实体类的相关信息。

### 模板管理
支持对模板的管理，可存储模板内容、输出路径等信息，方便根据不同的需求生成不同格式的代码。

### 多对多关系支持
通过 `SolutionsTemplate` 实体实现 `Solutions` 和 `Template` 之间的多对多关系，增强了模板与解决方案之间的关联灵活性。

## 技术栈
- **框架**：.NET 9.0、ASP.NET Core
- **序列化**：`Microsoft.AspNetCore.Mvc.NewtonsoftJson`
- **代码分析**：`Microsoft.CodeAnalysis.CSharp`
- **模板引擎**：`RazorEngine.NetCore`

## 安装与使用

### 环境要求
- .NET 9.0 SDK
- Visual Studio 2022 或更高版本

### 安装依赖
克隆项目到本地后，使用以下命令恢复项目依赖：
```bash
dotnet restore
```

### 运行项目
在项目根目录下执行以下命令启动项目：
```bash
dotnet run
```

### 使用示例
由于不清楚具体的使用方式，你可以补充代码生成的命令或者 API 调用示例，以下为示例格式：
```bash
# 假设通过命令行生成代码
dotnet run -- --input ./Entities --template ./Templates/DefaultTemplate.cshtml --output ./GeneratedCode
```

## 代码结构
```plaintext
CodeFirstGenerator/
├── .config/                # 配置文件目录
├── .git/                   # Git 版本控制目录
├── Controllers/            # 控制器目录（若有 API 相关功能）
│   ├── ...
├── Data/                   # 数据访问层目录（若有数据库操作）
│   ├── IdSettingInterceptor.cs
│   └── ...
├── Dtos/                   # 数据传输对象目录
│   ├── EntityInfoGetListInput.cs
│   └── ...
├── Entities/               # 实体类目录
│   ├── IEntity.cs
│   └── ...
├── Migrations/             # 若有数据库迁移，可保留该目录说明
│   └── ...
├── Services/               # 服务层目录
│   ├── IGeneratorService.cs
│   └── ...
├── CodeFirstGenerator.csproj # 项目文件
├── CodeFirstGenerator.sln  # 解决方案文件
└── ...
```

## 核心代码分析

### 实体接口定义
`IEntity` 接口为实体类提供了统一的 `Id` 属性定义：
```csharp:c:\Users\11413\Documents\Projects\CodeFirstGenerator\Entities\IEntity.cs
public interface IEntity
{
    long Id { get; set; }
}
```

### 数据传输对象
`EntityInfoGetListInput` 用于获取实体类列表信息：
```csharp:c:\Users\11413\Documents\Projects\CodeFirstGenerator\Dtos\EntityInfoGetListInput.cs
public class EntityInfoGetListInput
{
    // 类的属性和方法
}
```

## 贡献指南
如果你想为 `CodeFirstGenerator` 项目做贡献，请按照以下步骤操作：
1. Fork 本仓库到你的 GitHub 账户。
2. 创建一个新的分支：`git checkout -b feature/your-feature-name`。
3. 提交你的更改：`git commit -m "Add your feature description"`。
4. 推送分支到你的仓库：`git push origin feature/your-feature-name`。
5. 在 GitHub 上创建一个 Pull Request。

## 许可证
本项目采用 [MIT 许可证](LICENSE)，详情请查看 `LICENSE` 文件。

## 联系信息
如果你在使用过程中遇到问题或有任何建议，欢迎通过以下方式联系我们：
- GitHub Issues: [https://github.com/your-repo/CodeFirstGenerator/issues](https://github.com/your-repo/CodeFirstGenerator/issues)
- 邮箱: your-email@example.com

请将 `your-repo` 和 `your-email@example.com` 替换为你实际的 GitHub 仓库地址和邮箱地址。 