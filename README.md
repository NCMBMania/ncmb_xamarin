# Xamarin SDK for NCMB

ニフクラ mobile backendのXamarin用SDKです。

## インストール

NuGet から NCMBClient パッケージをインストールしてください。

## 使い方

### 初期化

```cs
var ncmb = new NCMB("ea5...265", "fe3...615");
```

### データストア

#### 保存

**同期処理**

```cs
// データストアの操作
var hello = ncmb.Object("Hello");
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
var hello = ncmb.Object("Hello");
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

#### データのアクセス

返却値は object 型なので、必要な型にキャストするか型引数を指定するバージョンを使用してください。

**文字列型の場合**

```cs
var str1 = (string) hello.Get("objectId")
var str2 = hello.Get<string>("objectId");
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
var query = ncmb.Query("Hello");
query.EqualTo("message", "Test message").EqualTo("number", 501);

var results = query.Find();
Console.WriteLine(results[0].Get("objectId"));

// 配列を検索
query.InString("message", new JArray("Test message"));
var results2 = query.Find();
Console.WriteLine(results2[0].Get("objectId"));

// 数値を使った検索
query.GreaterThan("number", 500);
var results3 = query.Find();
Console.WriteLine(results3[0].Get("objectId"));

// 日付を使った検索
var query2 = ncmb.Query("Hello");
query2.greaterThan("time", DateTime.Parse("2020-07-10T08:40:00"));
var results4 = query2.Find();
Console.WriteLine(results4[0].Get("objectId"));
```

**非同期処理の場合**

```cs
// 文字列、数字の検索
var query = ncmb.Query("Hello");
query.EqualTo("message", "Test message").EqualTo("number", 501);

var results = await query.FindAsync();
Console.WriteLine(results[0].Get("objectId"));

// 配列を検索
query.InString("message", new JArray("Test message"));
var results2 = await query.FindAsync();
Console.WriteLine(results2[0].Get("objectId"));

// 数値を使った検索
query.GreaterThan("number", 500);
var results3 = await query.FindAsync();
Console.WriteLine(results3[0].Get("objectId"));

// 日付を使った検索
var query2 = ncmb.Query("Hello");
query2.greaterThan("time", DateTime.Parse("2020-07-10T08:40:00"));
var results4 = await query2.FindAsync();
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

## ライセンス

MIT License

