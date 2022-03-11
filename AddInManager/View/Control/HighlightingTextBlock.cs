using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RevitAddinManager.View.Control
{
  /// <summary>
  /// 带高亮的文本框<see cref="TextBlock"/>
  /// </summary>
  [TemplatePart(Name = HighlighttextblockName, Type = typeof(TextBlock))]
  public class HighlightingTextBlock : System.Windows.Controls.Control
  {
    private const string HighlighttextblockName = "PART_HighlightTextblock";

    private static readonly DependencyPropertyKey MatchCountPropertyKey
        = DependencyProperty.RegisterReadOnly("MatchCount",
                                              typeof(int),
                                              typeof(HighlightingTextBlock),
                                              new PropertyMetadata(0));

    public static readonly DependencyProperty MatchCountProperty
        = MatchCountPropertyKey.DependencyProperty;

    /// <summary>
    /// 高亮的文本依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightTextProperty
      = DependencyProperty.Register("HighlightText",
                                    typeof(string),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(string.Empty, OnHighlightTextPropertyChanged));

    /// <summary>
    /// 文本依赖属性
    /// </summary>
    public static readonly DependencyProperty TextProperty
      = TextBlock.TextProperty.AddOwner(typeof(HighlightingTextBlock),
                                        new PropertyMetadata(string.Empty, OnTextPropertyChanged));

    /// <summary>
    /// 折行依赖属性
    /// </summary>
    public static readonly DependencyProperty TextWrappingProperty
      = TextBlock.TextWrappingProperty.AddOwner(typeof(HighlightingTextBlock),
                                                new PropertyMetadata(TextWrapping.NoWrap));

    /// <summary>
    /// 修整文本依赖属性
    /// </summary>
    public static readonly DependencyProperty TextTrimmingProperty
      = TextBlock.TextTrimmingProperty.AddOwner(typeof(HighlightingTextBlock),
                                                new PropertyMetadata(TextTrimming.None));

    /// <summary>
    /// 高亮前景颜色依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightForegroundProperty
      = DependencyProperty.Register("HighlightForeground",
                                    typeof(Brush),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(Brushes.White));

    /// <summary>
    /// 高亮<see cref="FontStyle"/>依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightFontStyleProperty
      = DependencyProperty.Register("HighlightFontStyle",
                                    typeof(FontStyle),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(FontStyles.Normal));

    /// <summary>
    /// 高亮<see cref="FontWeight"/>依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightFontWeightProperty
      = DependencyProperty.Register("HighlightFontWeight",
                                    typeof(FontWeight),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(FontWeights.Normal));

    /// <summary>
    /// 高亮文本修饰<see cref="TextDecorations"/>依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightTextDecorationsProperty
      = DependencyProperty.Register("HighlightTextDecorations",
                                    typeof(TextDecorationCollection),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(TextDecorations.Underline));

    /// <summary>
    /// 高亮背景颜色依赖属性
    /// </summary>
    public static readonly DependencyProperty HighlightBackgroundProperty
      = DependencyProperty.Register("HighlightBackground",
                                    typeof(Brush),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(Brushes.Blue));

    //Done 高亮部分匹配比较 是否忽略大小写
    /// <summary>
    /// 高亮文本匹配规则
    /// </summary>
    public static readonly DependencyProperty HighlightStringComparisonProperty
      = DependencyProperty.Register("HighlightStringComparison",
                                    typeof(StringComparison),
                                    typeof(HighlightingTextBlock),
                                    new PropertyMetadata(StringComparison.InvariantCultureIgnoreCase));

    /// <summary>
    /// 文本框
    /// </summary>
    private TextBlock highlightTextBlock;

    static HighlightingTextBlock()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightingTextBlock),
          new FrameworkPropertyMetadata(typeof(HighlightingTextBlock)));
    }

    /// <summary>
    /// 匹配数量
    /// </summary>
    public int MatchCount
    {
      get => (int)GetValue(MatchCountProperty);
      protected set => SetValue(MatchCountPropertyKey, value);
    }

    /// <summary>
    /// 高亮背景颜色
    /// </summary>
    public Brush HighlightBackground
    {
      get => (Brush)GetValue(HighlightBackgroundProperty);
      set => SetValue(HighlightBackgroundProperty, value);
    }

    /// <summary>
    /// 高亮文本匹配规则
    /// </summary>
    public StringComparison HighlightStringComparison
    {
      get => (StringComparison)GetValue(HighlightStringComparisonProperty);
      set => SetValue(HighlightStringComparisonProperty, value);
    }

    /// <summary>
    /// 高亮前景颜色
    /// </summary>
    public Brush HighlightForeground
    {
      get => (Brush)GetValue(HighlightForegroundProperty);
      set => SetValue(HighlightForegroundProperty, value);
    }

    /// <summary>
    /// 高亮<see cref="FontStyle"/>
    /// </summary>
    public FontStyle HighlightFontStyle
    {
      get => (FontStyle)GetValue(HighlightFontStyleProperty);
      set => SetValue(HighlightFontStyleProperty, value);
    }

    /// <summary>
    /// 高亮<see cref="FontWeight"/>
    /// </summary>
    public FontWeight HighlightFontWeight
    {
      get => (FontWeight)GetValue(HighlightFontWeightProperty);
      set => SetValue(HighlightFontWeightProperty, value);
    }

    /// <summary>
    /// 高亮文本修饰<see cref="TextDecorations"/>
    /// </summary>
    public TextDecorationCollection HighlightTextDecorations
    {
      get => (TextDecorationCollection)GetValue(HighlightTextDecorationsProperty);
      set => SetValue(HighlightTextDecorationsProperty, value);
    }

    /// <summary>
    /// 高亮的文本
    /// </summary>
    public string HighlightText
    {
      get => (string)GetValue(HighlightTextProperty);
      set => SetValue(HighlightTextProperty, value);
    }

    /// <summary>
    /// 文本
    /// </summary>
    public string Text
    {
      get => (string)GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 折行
    /// </summary>
    public TextWrapping TextWrapping
    {
      get => (TextWrapping)GetValue(TextWrappingProperty);
      set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>
    /// 修整文本
    /// </summary>
    public TextTrimming TextTrimming
    {
      get => (TextTrimming)GetValue(TextTrimmingProperty);
      set => SetValue(TextTrimmingProperty, value);
    }

    /// <summary>
    /// <see cref="HighlightText"/>属性改变时回调函数
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnHighlightTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textblock = (HighlightingTextBlock)d;
      textblock.ProcessTextChanged(textblock.Text, e.NewValue as string);
    }

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textblock = (HighlightingTextBlock)d;
      textblock.ProcessTextChanged(e.NewValue as string, textblock.HighlightText);
    }

    /// <summary>
    /// 文本改变时的处理函数
    /// </summary>
    /// <param name="mainText"></param>
    /// <param name="highlightText"></param>
    private void ProcessTextChanged(string mainText, string highlightText)
    {
      if (highlightTextBlock == null)
      {
        return;
      }

      highlightTextBlock.Inlines.Clear();
      SetValue(MatchCountPropertyKey, 0);

      if (highlightTextBlock == null || string.IsNullOrWhiteSpace(mainText))
      {
        return;
      }

      if (string.IsNullOrWhiteSpace(highlightText))
      {
        var completeRun = new Run(mainText);
        highlightTextBlock.Inlines.Add(completeRun);
        return;
      }

      if (highlightText.Contains(" ") || highlightText.Contains(","))
      {
        var textArray = highlightText.Split(new string[] { " ", "," },
                                            StringSplitOptions.RemoveEmptyEntries);
        ColorBrushTextParts(mainText, textArray.ToList());
      }
      else
      {
        ColorBrushTextParts(mainText, new List<string>() { highlightText });
      }
    }

    /// <summary>
    /// 对匹配的文字着色
    /// </summary>
    /// <param name="mainText">文字</param>
    /// <param name="highlightText">匹配的需要亮显的文字</param>
    private void ColorBrushTextPart(string mainText, string highlightText)
    {
      var find = 0;
      var searchTextLength = highlightText.Length;
      while (true)
      {
        var oldFind = find;
        find = mainText.IndexOf(highlightText, find, HighlightStringComparison);
        if (find == -1)
        {
          highlightTextBlock.Inlines.Add(
              oldFind > 0
                  ? GetRunForText(mainText.Substring(oldFind, mainText.Length - oldFind), false)
                  : GetRunForText(mainText, false));
          break;
        }

        if (oldFind == find)
        {
          highlightTextBlock.Inlines.Add(GetRunForText(mainText.Substring(oldFind, searchTextLength), true));
          SetValue(MatchCountPropertyKey, MatchCount + 1);
          find = find + searchTextLength;
          continue;
        }

        highlightTextBlock.Inlines.Add(GetRunForText(mainText.Substring(oldFind, find - oldFind), false));
      }
    }

    /// <summary>
    /// 对匹配的文字列表着色 即多个条件
    /// </summary>
    /// <param name="mainText">文字</param>
    /// <param name="highlightTextList">匹配的需要亮显的文字</param>
    private void ColorBrushTextParts(string mainText, List<string> highlightTextList)
    {
      if (highlightTextList.Count == 1)
      {
        ColorBrushTextPart(mainText, highlightTextList[0]);
        return;
      }

      var find = 0;
      while (true)
      {
        var oldFind = find;

        //能匹配的 需要亮显的字符串
        var canBeMatchTextList =
            (from item in highlightTextList
             let index = mainText.IndexOf(item, find, HighlightStringComparison)
             where index >= 0
             orderby index
             select item)
            .ToList();

        if (canBeMatchTextList.Count == 0)
        {
          //字符串的最后不着色
          highlightTextBlock.Inlines.Add(GetRunForText(mainText.Substring(oldFind, mainText.Length - oldFind), false));
          break;
        }

        string highlightText = canBeMatchTextList.First();
        var searchTextLength = highlightText.Length;

        find = mainText.IndexOf(highlightText, find, HighlightStringComparison);
        if (find == -1)
        {
          highlightTextBlock.Inlines.Add(
              oldFind > 0
                  ? GetRunForText(mainText.Substring(oldFind, mainText.Length - oldFind), false)
                  : GetRunForText(mainText, false));
          break;
        }

        if (oldFind == find)
        {
          highlightTextBlock.Inlines.Add(GetRunForText(mainText.Substring(oldFind, searchTextLength), true));
          SetValue(MatchCountPropertyKey, MatchCount + 1);
          find = find + searchTextLength;
          continue;
        }

        highlightTextBlock.Inlines.Add(GetRunForText(mainText.Substring(oldFind, find - oldFind), false));
      }
    }

    /// <summary>
    /// 为匹配的文字部分添加着色
    /// </summary>
    /// <param name="text">匹配的文字部分</param>
    /// <param name="isHighlighted">是否亮显（着色）</param>
    /// <returns></returns>
    private Run GetRunForText(string text, bool isHighlighted)
    {
      var textRun = new Run(text)
      {
        Foreground = isHighlighted ? HighlightForeground : Foreground,
        Background = isHighlighted ? HighlightBackground : Background,

        //Done 高亮部分斜体 FontStyle
        FontStyle = isHighlighted ? HighlightFontStyle : FontStyle,

        //Done 高亮部分加粗 FontWeight="Bold"
        FontWeight = isHighlighted ? HighlightFontWeight : FontWeight,

        //Done 高亮部分添加下划线
        TextDecorations = isHighlighted ? HighlightTextDecorations : highlightTextBlock.TextDecorations,
      };

      return textRun;
    }

    public override void OnApplyTemplate()
    {
      highlightTextBlock = GetTemplateChild(HighlighttextblockName) as TextBlock;
      if (highlightTextBlock == null)
      {
        return;
      }

      ProcessTextChanged(Text, HighlightText);
    }
  }

}
