# Xamarin SDK for NCMB

ニフクラ mobile backendのXamarin用SDKです。

## インストール

コードをダウンロードして、自分のXamarinプロジェクトに取り込んでください。

**注意**

- ネームスペースが `ncmb_xamarin` になっていますので、適宜修正してください
- 依存ライブラリとしてJSON.NETを追加してください

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
hello.set("message", "Hello world");
hello.set("number", 100);
hello.set("bol", true);
var ary = new JArray();
ary.Add("test1");
ary.Add("test2");
hello.set("array", ary);
var obj = new JObject();
obj["test1"] = "Hello";
obj["test2"] = 100;
hello.set("obj", obj);
hello.set("time", DateTime.Now);
hello.save();
```

#### データのアクセス

返却値は JToken なので、必要な型にキャストしてください。

**文字列型の場合**

```cs
(string) hello.get("objectId")
```

**配列の場合**

```cs
var ary = (JArray) hello.get("array");
```

## ライセンス

MIT License

