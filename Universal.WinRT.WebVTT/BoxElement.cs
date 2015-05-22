using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.Media.WebVTT
{
    public sealed class BoxElement : Control
    {
        Panel container;
        readonly List<TextBlock> blocks = new List<TextBlock>();

        public BoxElement()
        {
            this.DefaultStyleKey = typeof(BoxElement);
        }

        static readonly DependencyProperty blockStyleProperty = DependencyProperty.Register("BlockStyle", typeof(Style), typeof(BoxElement), new PropertyMetadata(null, (d, e) => ((BoxElement)d).OnBlockStyleChanged(e.NewValue as Style)));
        public static DependencyProperty BlockStyleProperty { get { return blockStyleProperty; } }

        public Style BlockStyle
        {
            get { return GetValue(blockStyleProperty) as Style; }
            set { SetValue(blockStyleProperty, value); }
        }

        void OnBlockStyleChanged(Style blockStyle)
        {
            foreach (var block in blocks)
            {
                block.Style = blockStyle;
            }
            if (container != null)
            {
                foreach (var block in container.Children.Cast<TextBlock>())
                {
                    block.Style = blockStyle;
                }
            }
        }

#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            container = GetTemplateChild("Container") as Panel;
            if (container != null)
            {
                foreach (var block in blocks)
                {
                    container.Children.Add(block);
                }
                blocks.Clear();
            }
        }

        public void AddBlock(TextBlock block)
        {
            if (BlockStyle != null) block.Style = BlockStyle;
            if (container != null)
            {
                container.Children.Add(block);
            }
            else
            {
                blocks.Add(block);
            }
        }

        public void RemoveBlock(TextBlock block)
        {
            if (container != null)
            {
                container.Children.Remove(block);
            }
            else
            {
                blocks.Remove(block);
            }
        }
    }
}
