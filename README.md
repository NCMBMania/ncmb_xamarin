# Xamarin SDK for NCMB

ニフクラ mobile backendのXamarin用SDKです。

## インストール

NuGet から NCMBClient パッケージをインストールしてください。

## 使い方

### 初期化

```cs
new NCMB("ea5...265", "fe3...615");
```

### 会員管理

#### 会員登録

**同期処理の場合**

```cs
var user = new NCMBUser();
user.Set("userName", "TestUser");
user.Set("password", "TestPass");
await user.SignUp();
```

**非同期処理の場合**

```cs
var user = new NCMBUser();
user.Set("userName", "TestUser");
user.Set("password", "TestPass");
await user.SignUpAsync();
```

#### ログイン（ID/パスワード）

**同期処理の場合**

```cs
var user = new NCMBUser();
user.Set("userName", "TestLogin");
user.Set("password", "TestLogin");
if (user.Login())
{
  // Login success
} else
{
  // Login Failure
}
```

**非同期処理の場合**

```cs
var user = new NCMBUser();
user.Set("userName", "TestLogin");
user.Set("password", "TestLogin");
if (await user.LoginAsync())
{
  // Login success
} else
{
  // Login Failure
}
```

#### 会員削除

**同期の場合**

```cs
user.Delete();
```

**非同期の場合**

```cs
await user.DeleteAsync();
```

#### ログアウト

```cs
NCMBUser.Logout();
```

### データストア

#### 保存

**同期処理**

```cs
// データストアの操作
var hello = new NCMBObject("Hello");
hello.Set("message", "Hello world");
hello.Set("number", 100);
hello.Set("bol", true);
var ary = new JArray();
ary.Add("test1");
ary.Add("test2");
hello.Set("array", ary);
var obj = new JObject();
obj["test1"] = "Hello";
obj["test2"] = 100;
hello.Set("obj", obj);
hello.Set("time", DateTime.Now);
hello.Save();
```

**非同期処理**

```cs
// データストアの操作
var hello = new NCMBObject("Hello");
hello.Set("message", "Hello world");
hello.Set("number", 100);
hello.Set("bol", true);
var ary = new JArray();
ary.Add("test1");
ary.Add("test2");
hello.Set("array", ary);
var obj = new JObject();
obj["test1"] = "Hello";
obj["test2"] = 100;
hello.Set("obj", obj);
hello.Set("time", DateTime.Now);
await hello.SaveAsync();
```

#### キーチェーンメソッドの利用

Setメソッドは自分自身を返すので、キーチェーンメソッドを利用できます。

```cs
hello.Set("message", "Hello world").Set("number", 100).Set("bol", true);
```

#### ACLの利用

権限管理（ACL）NCMBAclを使います。

```cs
var message = "Hello, world";
var item = new NCMBObject("DataStoreTest");
item.Set("message", message);
var acl = new NCMBAcl();
acl.SetPublicReadAccess(true);
acl.SetPublicWriteAccess(false);
acl.SetRoleReadAccess("admin", true);
acl.SetRoleWriteAccess("admin", true);
item.SetAcl(acl);
item.Save();
```

用意されているのは次のメソッドです。

- SetPublicReadAccess(Boolean bol)
- SetPublicWriteAccess(Boolean bol)
- SetUserReadAccess(NCMBUser user, Boolean bol)
- SetUserWriteAccess(NCMBUser user, Boolean bol)
- SetRoleReadAccess(String name, Boolean bol)
- SetRoleWriteAccess(String name, Boolean bol)

#### データのアクセス

返却値は object 型なので、必要な型にキャストするか型引数を指定するバージョンを使用してください。

**文字列型の場合**

```cs
var str1 = (string) hello.Get("objectId");
var str2 = hello.Get<string>("objectId");
var str3 = hello.Get("objectId").ToString());
```

**配列の場合**

```cs
var ary1 = (JArray) hello.Get("array");
var ary2 = hello.Get<JArrat>("array");
```

#### 検索

**同期処理の場合**

```cs
// 文字列、数字の検索
var query = new NCMBQuery("Hello");
query.EqualTo("message", "Test message").EqualTo("number", 501);

var results = query.FetchAll();
Console.WriteLine(results[0].Get("objectId"));

// 配列を検索
query.InString("message", new JArray("Test message"));
var results2 = query.FetchAll();
Console.WriteLine(results2[0].Get("objectId"));

// 数値を使った検索
query.GreaterThan("number", 500);
var results3 = query.FetchAll();
Console.WriteLine(results3[0].Get("objectId"));

// 日付を使った検索
var query2 = ncmb.Query("Hello");
query2.greaterThan("time", DateTime.Parse("2020-07-10T08:40:00"));
var results4 = query2.FetchAll();
Console.WriteLine(results4[0].Get("objectId"));
```

**非同期処理の場合**

```cs
// 文字列、数字の検索
var query = new NCMBQuery("Hello");
query.EqualTo("message", "Test message").EqualTo("number", 501);

var results = await query.FetchAllAsync();
Console.WriteLine(results[0].Get("objectId"));

// 配列を検索
query.InString("message", new JArray("Test message"));
var results2 = await query.FetchAllAsync();
Console.WriteLine(results2[0].Get("objectId"));

// 数値を使った検索
query.GreaterThan("number", 500);
var results3 = await query.FetchAllAsync();
Console.WriteLine(results3[0].Get("objectId"));

// 日付を使った検索
var query2 = ncmb.Query("Hello");
query2.greaterThan("time", DateTime.Parse("2020-07-10T08:40:00"));
var results4 = await query2.FetchAllAsync();
Console.WriteLine(results4[0].Get("objectId"));
```


**その他のオペランド**

- EqualTo(string name, object value)
- NotEqualTo(string name, object value)
- LessThan(string name, object value)
- LessThanOrEqualTo(string name, object value)
- GreaterThan(string name, object value)
- GreaterThanOrEqualTo(string name, object value)
- InString(string name, object value)
- NotInString(string name, object value)
- Exists(string name, bool value = true)
- RegularExpressionTo(string name, object value)
- InArray(string name, object value)
- NotInArray(string name, object value)
- AllInArray(string name, object value)

### リレーション

#### リレーションを作成して保存

```cs
var item1 = new NCMBObject("RelationTest");
item1.Set("name", "item1").Save();
var item2 = new NCMBObject("RelationTest");
item2.Set("name", "item2").Save();

var relation = new NCMBRelation();
relation.Add(item1).Add(item2);

var item3 = new NCMBObject("RelationMaster");
item3.Set("relation", relation).Save();
```

#### リレーションからデータを削除して保存

```cs
var item1 = new NCMBObject("RelationTest");
item1.Set("name", "item1").Save();
var item2 = new NCMBObject("RelationTest");
item2.Set("name", "item2").Save();

var relation = new NCMBRelation();
relation.Add(item1).Add(item2);

var item3 = new NCMBObject("RelationMaster");
item3.Set("relation", relation).Save();

relation = new NCMBRelation();
relation.Remove(item1);
item3.Set("relation", relation).Save();
```

### ポインター

#### ポインターとしてオブジェクトを保存

```cs
var item1 = new NCMBObject("QueryTest");
item1.Set("message", "Test message");
item1.Set("number", 500);
await item1.SaveAsync();

var item2 = new NCMBObject("QueryTest");
item2.Set("message", "Test message");
item2.Set("number", 500);
item2.Set("obj", item1);
await item2.SaveAsync();

var query = new NCMBQuery("QueryTest");
query.EqualTo("objectId", item2.Get("objectId")).Include("obj");
var obj = await query.FetchAsync();
obj.Get("objectId" ).ToString() == item2.Get("objectId").ToString();
// => true
((NCMBObject) obj.Get("obj")).Get("objectId").ToString() == item1.Get("objectId").ToString();
// => true
```

## ロール

### ロールの作成と削除

```cs
var role = new NCMBRole();
role.Set("roleName", "admin");
role.Save();
role.Delete();
```

### 子ロールの作成

```cs
role1.Set("roleName", "role1");
role1.Save();

var role2 = new NCMBRole();
role2.Set("roleName", "role2");
role2.Save();

role1.AddRole(role2).Save();

role1.Fetch();

var roles = role1.FetchRole();

role2.Get("roleName").ToString() == roles[0].Get("roleName").ToString()
// => true
```

### ロールへのユーザの追加

```cs
var acl = new NCMBAcl();
acl.SetPublicWriteAccess(true);

var user1 = new NCMBUser();
var userName = "TestLogin1";
var password = "TestPass";
user1.Set("userName", userName);
user1.Set("password", password);
user1.SignUp();
var user = NCMBUser.Login(userName, password);
user.SetAcl(acl);
user.Save();

var user2 = new NCMBUser();
userName = "TestLogin2";
password = "TestPass";
user2.Set("userName", userName);
user2.Set("password", password);
user2.SignUp();

user = NCMBUser.Login(userName, password);
user.SetAcl(acl);
user.Save();

var role1 = new NCMBRole();
role1.Set("roleName", "role5");
role1.Save();

role1.AddUser(user1).AddUser(user2).Save();

role1.Fetch();

var users = role1.FetchUser();
Assert.AreEqual(2, users.Length);
role1.ClearOperation();
role1.RemoveUser(user1).Save();
users = role1.FetchUser();
Assert.AreEqual(1, users.Length);
```

## ライセンス

MIT License

