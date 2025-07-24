# .Net 6 銀行後端系統

- [Features](#features)
- [QuickStart](#quickstart)
- [API-Test](#api-test)
  - [API for User](#api-for-user)
  - [API for Admin](#api-for-admin)

# Features
| Features |
|------|
| MVC |
| 路由 |
| RESTful_API |
| JWT 身分驗證 |
| DTO |
| EF Core + SQLite |
| 權限管理 |


# QuickStart
### Development

```bash
dotnet run
```
#### server run on https://localhost:5295

# API Test
## API for User
#### 1-1. 用戶註冊
```bash
curl -k -X POST "https://localhost:5295/api/user/register" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"123456", "email":"tester001@gmail.com"}'
```
#### 1-2. 驗證帳號，驗證完畢後即可登入
```bash
curl -k -X POST "https://localhost:5295/api/user/verify?username=tester001"
```
#### 1-3. 用戶登入並取得json web token
```bash
curl -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"123456"}'
```
#### 1-4. 用戶查詢自己的資料(使用登入時拿到的token)
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/who-am-i -H "authorization: Bearer <token>"
```
## API for Admin 
#### 2-1. 管理員登入，預設已驗證
```bash
curl -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"username":"admin", "password":"admin-password"}'
```
#### 2-2. 查詢某用戶資料(以名字查詢)，要放入token
```bash
curl -i -s -k -X GET "https://localhost:5295/api/user/get-user?username=tester001" -H "authorization: Bearer <token>"
```
#### 2-3. 查詢用戶資料(以信箱查詢)，要放入token
```bash
curl -s -i -k -X GET "https://localhost:5295/api/user/get-user?email=tester001@gmail.com" -H "authorization: Bearer <token>"
```
#### 2-4. 查詢所有用戶資料，要放入token
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/get-all-users -H "authorization: Bearer <token>"
```
