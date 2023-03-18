数据迁移脚本(power shell)

```
 1.dotnet add package Microsoft.EntityFrameworkCore.Design
 2.dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
 3.dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
 4.dotnet ef migrations add Init_migration -c ApplicationDbContext  -o Data/
 5.dotnet ef database update --context PersistedGrantDbContext
 6.dotnet ef database update --context ConfigurationDbContext
 7.dotnet ef database update --context ApplicationDbContext


```

