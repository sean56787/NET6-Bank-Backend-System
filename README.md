# .Net 6 銀行後端系統
### 用戶註冊及登入 / 用戶存提款、轉帳及交易紀錄查詢 / 管理員權限功能
- [Features](#features)
- [Tools & Versions](#tools--versions)
- [QuickStart](#quickstart)
- API
  - [API - 普通用戶](#api-for-普通用戶)
  - [API - 用戶存提款、轉帳](#api-for-用戶存提款轉帳)
  - [API - 後臺管理員](#api-for-後臺管理員)

# Features
| Features |
|------|
| `MVC 架構` |
| `路由 、 RESTful_API` |
| `JWT 身分驗證` |
| `EF Core + SQLite` |

# Tools & Versions
| 名稱 | 版本 |
|------|------|
| C# | `10.0` |
| .Net6 | `6.0.428` |
| BCrypt | `4.0.3` |
| JwtBearer | `6.0.25` |
| EFcore SQLite | `6.0.25` |

# QuickStart
### 在專案根目錄下執行
```bash
dotnet run
```
#### 伺服器應運行於 https://localhost:5295

# 用戶 | 管理員
| 身分 | features |
|------|------|
| 用戶 | `註冊帳戶`、`驗證帳戶`、`登入帳戶`、`存款`、`提款`、`轉帳` |
| 管理員 | `增刪查改用戶資料`、`查詢交易明細`、`凍結或刪除用戶` |
---

# API 
> 💡 **請在 Git Bash 環境執行以下 API**
## API for 普通用戶
> #### 1-1. 用戶註冊帳號
```bash
curl -i -s -k -X POST "https://localhost:5295/api/user/register" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"123456", "email":"tester001@gmail.com"}'
```
> #### 1-2. 驗證帳號，帳號經驗證才可登入
```bash
curl -i -s -k -X POST "https://localhost:5295/api/user/verify?email=tester001@gmail.com"
```
> #### 1-3. 用戶登入並取得 JWT token，token 用於身分驗證
```bash
curl -i -s -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"UserId":"2", "password":"123456"}'
```
> #### 1-4. 用戶查詢自己的詳細資訊(在 \<token\> 處放入剛才取得的 token)
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/who-am-i -H "authorization: Bearer <token>"
```
---
## API for 用戶存提款、轉帳
> #### 2-1. 用戶存款 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/balance/deposit" -H "Content-Type: application/json" -d '{"userId":"2", "amount":"1000", "Type":"Deposit", "description":"a deposit note"}' -H "authorization: Bearer <token>"
```
> #### 2-2. 用戶提款 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/balance/withdraw" -H "Content-Type: application/json" -d '{"userId":"2", "Amount":"100", "Type":"Withdraw", "description":"a withdraw note"}' -H "authorization: Bearer <token>"
```
> #### 2-3. 用戶之間轉帳 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/balance/transfer" -H "Content-Type: application/json" -d '{"FromUserId":"2","ToUserId":"1", "Amount":"100", "Type":"transfer", "description":"a transfer to userId=1"}' -H "authorization: Bearer <token>"
```
---
## API for 後臺管理員
> #### 3-1. 管理員登入，預設已驗證身分，可直接登入
```bash
curl -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"UserId":"1", "password":"admin-password"}'
```
> #### 3-2. 查詢某用戶資料(以userId查詢) (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X GET "https://localhost:5295/api/user/get-user?userId=2" -H "authorization: Bearer <token>"
```
> #### 3-3. 查詢某用戶資料(以email查詢) (在 \<token\> 處放入 token)
```bash
curl -s -i -k -X GET "https://localhost:5295/api/user/get-user?email=tester001@gmail.com" -H "authorization: Bearer <token>"
```
> #### 3-4. 查詢所有用戶資料 (在 \<token\> 處放入 token)
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/get-all-users -H "authorization: Bearer <token>"
```
> #### 3-5. 更新用戶資料 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X PUT "https://localhost:5295/api/admin/update-user" -H "Content-Type: application/json" -d '{"userId":"2", "username":"tester001-updated", "password":"456789", "email":"tester001-updated@gmail.com", "role":"user"}' -H "authorization: Bearer <token>"
```
> #### 3-6. 手動建立用戶資料 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/admin/create-user" -H "Content-Type: application/json" -d '{"username":"A-User-Create-By-Admin", "password":"123456", "email":"AUserCreateByAdmin@gmail.com", "role":"admin"}' -H "authorization: Bearer <token>"
```
> #### 3-7. 凍結某用戶 (在 \<token\> 處放入 token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/admin/frozen-user" -H "Content-Type: application/json" -d '{"userId":"2"}' -H "authorization: Bearer <token>"
```
> #### 3-8. 刪除某用戶 (在 \<token\> 處放入 token) 
```bash
curl -i -s -k -X DELETE "https://localhost:5295/api/admin/delete-user" -H "Content-Type: application/json" -d '{"userId":"2"}' -H "authorization: Bearer <token>"
```
> #### 3-9. 變動某用戶餘額 (在 \<token\> 處放入 token) 
```bash
curl -i -s -k -X POST "https://localhost:5295/api/balance/adjust" -H "Content-Type: application/json" -d '{"userId":"2", "Amount":"99", "Type":"AdjustBySystem", "description":"adjust by system"}' -H "authorization: Bearer <token>"
```
> #### 3-10. 查詢某用戶交易資料 (在 \<token\> 處放入 token) 
```bash
curl -i -s -k -X Post "https://localhost:5295/api/balance/user-transactions" -H "Content-Type: application/json" -d '{"userId":"2", "startDate":"2025-01-01T00:00:00","endDate":"2025-12-30T23:59:59","Page":"1","Pagesize":"10"}' -H "authorization: Bearer <token>"
```










