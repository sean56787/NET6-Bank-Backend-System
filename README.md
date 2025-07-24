# .Net 6 銀行後端系統
### 用戶註冊及登入 / 管理員權限 / 用戶餘額異動及交易紀錄查詢
- [Features](#features)
- [QuickStart](#quickstart)
- [API-Test](#api-test)
  - [API for User](#api-for-user)
  - [API for Admin](#api-for-admin)
  - [API for Transactions](#api-for-transactions)

# Features
| Features |
|------|
| MVC 架構 |
| 路由 + RESTful_API |
| JWT 身分驗證 |
| DTO 自訂輸入輸出 |
| EF Core + SQLite 建立資料庫 |


# QuickStart
### Development

```bash
dotnet run
```
#### server run on https://localhost:5295

---
# API Test
## API for User 普通用戶層
#### 1-1. 用戶註冊
```bash
curl -k -X POST "https://localhost:5295/api/user/register" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"123456", "email":"tester001@gmail.com"}'
```
#### 1-2. 驗證帳號，驗證完畢後即可登入
```bash
curl -k -X POST "https://localhost:5295/api/user/verify?username=tester001"
```
#### 1-3. 用戶登入並取得 JWT token(驗證身分用)
```bash
curl -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"123456"}'
```
#### 1-4. 用戶查詢自己的資料(在 \<token\> 處放入剛才取得的 JWT token)
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/who-am-i -H "authorization: Bearer <token>"
```
---
## API for Admin 管理員權限
#### 2-1. 管理員登入，預設已驗證身分，可直接登入
```bash
curl -k -X POST "https://localhost:5295/api/user/login" -H "Content-Type: application/json" -d '{"username":"admin", "password":"admin-password"}'
```
#### 2-2. 查詢某用戶資料(以名字查詢) (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X GET "https://localhost:5295/api/user/get-user?username=tester001" -H "authorization: Bearer <token>"
```
#### 2-3. 查詢用戶資料(以信箱查詢) (請在 \<token\> 處放入 JWT token)
```bash
curl -s -i -k -X GET "https://localhost:5295/api/user/get-user?email=tester001@gmail.com" -H "authorization: Bearer <token>"
```
#### 2-4. 查詢所有用戶資料 (請在 \<token\> 處放入 JWT token)
```bash
curl -s -i -k -X GET https://localhost:5295/api/user/get-all-users -H "authorization: Bearer <token>"
```
#### 2-5. 更新用戶資料 (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X PUT "https://localhost:5295/api/admin/update-user" -H "Content-Type: application/json" -d '{"username":"tester001", "password":"456789", "email":"tester001_updated@gmail.com", "role":"admin"}' -H "authorization: Bearer <token>"
```
#### 2-6. 手動建立用戶 (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/admin/create-user" -H "Content-Type: application/json" -d '{"username":"tester002", "password":"123456", "email":"tester002@gmail.com", "role":"user"}' -H "authorization: Bearer <token>"
```
#### 2-7. 凍結帳戶 (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/admin/frozen-user" -H "Content-Type: application/json" -d '{"username":"tester001"}' -H "authorization: Bearer <token>"
```
#### 2-8. 刪除帳戶 (請在 \<token\> 處放入 JWT token) 
```bash
curl -i -s -k -X POST "https://localhost:5295/api/admin/delete-user" -H "Content-Type: application/json" -d '{"username":"tester001"}' -H "authorization: Bearer <token>"
```
---
## API for Transactions 餘額異動
#### 3-1. 餘額異動 (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X POST "https://localhost:5295/api/balance/adjust" -H "Content-Type: application/json" -d '{"username":"admin", "Amount":"999", "balanceType":"ManualAdjust", "note":"test"}' -H "authorization: Bearer <token>"
```
#### 3-2. 查詢轉帳紀錄 (請在 \<token\> 處放入 JWT token)
```bash
curl -i -s -k -X Post "https://localhost:5295/api/balance/user-transactions" -H "Content-Type: application/json" -d '{"userId":"1", "operator":"admin", "balanceType":"ManualAdjust", "startDate":"2025-07-01T00:00:00","endDate":"2025-09-01T23:59:59","Page":"1","Pagesize":"10"}' -H "authorization: Bearer <token>"
```











