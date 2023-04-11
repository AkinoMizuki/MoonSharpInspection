# MoonSharpInspection
C#(.NET 5)でのMoonSharp(ver2.0.0)の実装メモです。

# 実装API
- print(value)
- printf(value)
- WinForm.sleep(msec)
- WinForm.LED(boolean)

# print(value)
GUI中央の黒いTextBoxに表示できます
※日本語対応
```lua
  print("文字表示")
```
# printf(value)
GUI中央の黒いTextBoxのprintf内の文字を描画します
※日本語対応
```lua
  printf("文字表示")
```

# WinForm.sleep(msec)
msecオーダーでウェイトを指定出来ます
```lua
 --100msウェイト
　WinForm.sleep(100)
```

# WinForm.LED(on_off)
フォームアプリケーションのLEDピクチャをONもしくはOFFできます
```lua
 --Onします
　WinForm.LED(true)
```

