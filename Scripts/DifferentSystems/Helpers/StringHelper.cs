using System;
using System.Linq;

public static class StringHelper
{
    public static string WrapWords(string text, int maxLength)
    {
        var words = text.Split(' ');
        var result = new System.Text.StringBuilder();
        int currentLength = 0;

        foreach (var word in words)
        {
            if (currentLength + word.Length + 1 <= maxLength)
            {
                if (currentLength > 0)
                {
                    result.Append(" ");
                    currentLength++;
                }
                result.Append(word);
                currentLength += word.Length;
            }
            else
            {
                result.AppendLine();
                result.Append(word);
                currentLength = word.Length;
            }
        }

        return result.ToString();
    }
}