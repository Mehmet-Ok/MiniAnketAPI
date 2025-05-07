# MiniAnketAPI

## Gereksinimler

Bu projeyi çalıştırabilmek için aşağıdaki yazılımlar yüklü olmalıdır:

- .NET 8.0 
- PostgreSQL 17 

## Kullanılan NuGet Paketleri

Projede kullanılan NuGet paketleri şunlardır:

- **BCrypt.Net-Next** v4.0.3: Şifre güvenliği için bcrypt algoritması.
- **Dapper** v2.1.66: Veritabanı sorguları için hızlı bir mikro ORM.
- **Microsoft.AspNetCore.Authentication.JwtBearer** v8.0.0: JWT (JSON Web Token) ile kimlik doğrulama.
- **Newtonsoft.Json** v13.0.3: JSON verilerini serileştirme ve deserileştirme.
- **Npgsql** v9.0.3: PostgreSQL veritabanı ile bağlantı kurmak için Npgsql .NET kütüphanesi.
- **Swashbuckle.AspNetCore** v6.6.2: API dokümantasyonu için Swagger entegrasyonu.

## Proje Kurulumu ve Kullanımı

### Adım 1: PostgreSQL Veritabanı Kurulumu

Proje, **PostgreSQL 17** kullanmaktadır. Veritabanını restore etmek için aşağıdaki adımları takip edebilirsiniz:

```
pg_restore -U postgres -d surveydb dbfiles/surveydb.backup
```
Yada
```
psql -U postgres -d surveydb -f dbfiles/init.sql
```

## Kullanıcı Kaydı ve Giriş Yapma

Projeyi kurduktan sonra API'yi kullanabilmek için aşağıdaki adımları takip edin:

1. **Swagger Arayüzünde Kullanıcı Kaydı Oluşturun**:
   - API'yi çalıştırdıktan sonra Swagger UI'ye gidin (örneğin, `http://localhost:{port}/swagger`).
   - **Kullanıcı kaydı** oluşturmak için ilgili **POST /api/register** endpoint'ini kullanın.
   
2. **Login API'sini Kullanarak Giriş Yapın**:
   - Kaydınızı tamamladıktan sonra, **POST /api/login** endpoint'ini kullanarak giriş yapın.
   - Giriş yaptıktan sonra, döndürülen **JWT**'yi kopyalayın.

3. **Authorization ile JWT ile Kimlik Doğrulaması Yapın**:
   - Swagger UI ekranının sağ üst kısmında bulunan **Authorization** butonuna tıklayın.
   - Açılan pencerede, "Bearer {kopyaladığınız JWT}" formatında token'ı girin.
     - Örnek: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

4. **API'leri Test Edin**:
   - JWT token ile kimlik doğrulaması yapıldıktan sonra, Swagger UI üzerinden güvenli API'leri test edebilirsiniz.


