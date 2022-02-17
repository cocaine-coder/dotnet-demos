# 技巧

## ObjectSettings

- 加载本地资源
``` csharp
 LoadSettings = new LoadSettings()
{
    BlockLocalFileAccess=false,
}
```

- 页码

```
PagesCount= true,
HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]" }
```


# 注意

- style
    - 不支持 flex

- script
    - 不支持lamda表达式
    - 不支持 `${xxx}` 字符串