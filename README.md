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

#### データのアクセス

返却値は JToken なので、必要な型にキャストしてください。

**文字列型の場合**

```cs
(string) hello.Get("objectId")
```

**配列の場合**

```cs
var ary = (JArray) hello.Get("array");
```

## ライセンス

MIT License

