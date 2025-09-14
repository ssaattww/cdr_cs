using System.Text;
using cdr_cs;

// 最小TDD: まずは未実装APIを呼び出す失敗テストを用意する。
// 目的: A1(String) の往復（Encode→Decode）で等価性を確認する。

static class Assert
{
    // 簡易アサート（外部パッケージ不要）
    public static void True(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    public static void Equal(string expected, string actual, string message)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new Exception($"{message}\nexpected='{expected}', actual='{actual}'");
    }
}

class Program
{
    static int Main()
    {
        try
        {
            // 未実装API（IStringInterop/Utf8StringInterop）を先に参照
            // 実装は次コミット以降で追加する想定。
            var interop = new Utf8StringInterop();

            // 正常系1: ASCII
            RoundTrip(interop, "hello");

            // 正常系2: 多バイト（日本語）
            RoundTrip(interop, "ほげ");

            // 正常系3: 空文字
            RoundTrip(interop, "");

            Console.WriteLine("OK");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL: {ex.Message}");
            return 1;
        }
    }

    // 往復テスト（Encode→Decode）
    static void RoundTrip(IStringInterop interop, string input)
    {
        // ここではUTF-8 + 先頭に長さ（仮）を想定しておくが、
        // 具体仕様はA1-1で確定させる。
        var encoded = interop.Encode(input);
        var decoded = interop.Decode(encoded);
        Assert.Equal(input, decoded, "RoundTrip mismatch");
    }
}

