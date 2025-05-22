# CodeFirstGenerator ����������

### ˵��
��README��AI�Զ����ɣ������ο���

## ��Ŀ����
`CodeFirstGenerator` ��һ������ .NET 9.0 ���������ô������ȣ�Code First���������������ù���רע������Ԥ�����ʵ���࣬��Ч�����ض����͵Ĵ��룬�Լ��ٿ����������ֶ���д�ظ�����Ĺ���������������Ч�ʡ�

## ��������
### ʵ���ദ��
- **�ӿڶ���**�������� `IEntity` �ӿڣ�Ϊʵ����ͳһ�ṩ `Id` ���ԣ���֤ʵ����Ĺ淶�ԡ�
- **��Ϣ��ȡ**���ܹ�ͨ�� `EntityInfoGetListInput` �����ݴ�������ȡʵ����������Ϣ��

### ģ�����
֧�ֶ�ģ��Ĺ����ɴ洢ģ�����ݡ����·������Ϣ��������ݲ�ͬ���������ɲ�ͬ��ʽ�Ĵ��롣

### ��Զ��ϵ֧��
ͨ�� `SolutionsTemplate` ʵ��ʵ�� `Solutions` �� `Template` ֮��Ķ�Զ��ϵ����ǿ��ģ����������֮��Ĺ�������ԡ�

## ����ջ
- **���**��.NET 9.0��ASP.NET Core
- **���л�**��`Microsoft.AspNetCore.Mvc.NewtonsoftJson`
- **�������**��`Microsoft.CodeAnalysis.CSharp`
- **ģ������**��`RazorEngine.NetCore`

## ��װ��ʹ��

### ����Ҫ��
- .NET 9.0 SDK
- Visual Studio 2022 ����߰汾

### ��װ����
��¡��Ŀ�����غ�ʹ����������ָ���Ŀ������
```bash
dotnet restore
```

### ������Ŀ
����Ŀ��Ŀ¼��ִ����������������Ŀ��
```bash
dotnet run
```

### ʹ��ʾ��
���ڲ���������ʹ�÷�ʽ������Բ���������ɵ�������� API ����ʾ��������Ϊʾ����ʽ��
```bash
# ����ͨ�����������ɴ���
dotnet run -- --input ./Entities --template ./Templates/DefaultTemplate.cshtml --output ./GeneratedCode
```

## ����ṹ
```plaintext
CodeFirstGenerator/
������ .config/                # �����ļ�Ŀ¼
������ .git/                   # Git �汾����Ŀ¼
������ Controllers/            # ������Ŀ¼������ API ��ع��ܣ�
��   ������ ...
������ Data/                   # ���ݷ��ʲ�Ŀ¼���������ݿ������
��   ������ IdSettingInterceptor.cs
��   ������ ...
������ Dtos/                   # ���ݴ������Ŀ¼
��   ������ EntityInfoGetListInput.cs
��   ������ ...
������ Entities/               # ʵ����Ŀ¼
��   ������ IEntity.cs
��   ������ ...
������ Migrations/             # �������ݿ�Ǩ�ƣ��ɱ�����Ŀ¼˵��
��   ������ ...
������ Services/               # �����Ŀ¼
��   ������ IGeneratorService.cs
��   ������ ...
������ CodeFirstGenerator.csproj # ��Ŀ�ļ�
������ CodeFirstGenerator.sln  # ��������ļ�
������ ...
```

## ���Ĵ������

### ʵ��ӿڶ���
`IEntity` �ӿ�Ϊʵ�����ṩ��ͳһ�� `Id` ���Զ��壺
```csharp:c:\Users\11413\Documents\Projects\CodeFirstGenerator\Entities\IEntity.cs
public interface IEntity
{
    long Id { get; set; }
}
```

### ���ݴ������
`EntityInfoGetListInput` ���ڻ�ȡʵ�����б���Ϣ��
```csharp:c:\Users\11413\Documents\Projects\CodeFirstGenerator\Dtos\EntityInfoGetListInput.cs
public class EntityInfoGetListInput
{
    // ������Ժͷ���
}
```

## ����ָ��
�������Ϊ `CodeFirstGenerator` ��Ŀ�����ף��밴�����²��������
1. Fork ���ֿ⵽��� GitHub �˻���
2. ����һ���µķ�֧��`git checkout -b feature/your-feature-name`��
3. �ύ��ĸ��ģ�`git commit -m "Add your feature description"`��
4. ���ͷ�֧����Ĳֿ⣺`git push origin feature/your-feature-name`��
5. �� GitHub �ϴ���һ�� Pull Request��

## ���֤
����Ŀ���� [MIT ���֤](LICENSE)��������鿴 `LICENSE` �ļ���

## ��ϵ��Ϣ
�������ʹ�ù�����������������κν��飬��ӭͨ�����·�ʽ��ϵ���ǣ�
- GitHub Issues: [https://github.com/your-repo/CodeFirstGenerator/issues](https://github.com/your-repo/CodeFirstGenerator/issues)
- ����: your-email@example.com

�뽫 `your-repo` �� `your-email@example.com` �滻Ϊ��ʵ�ʵ� GitHub �ֿ��ַ�������ַ�� 