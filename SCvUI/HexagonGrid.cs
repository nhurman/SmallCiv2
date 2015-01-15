using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SCvUI
{
    class HexagonGrid : Grid
    {
        /**
         * If the length of 1 side of the hexagon = S, then:
         * Width = 2 x S
         * Height = S x SQRT(3)
         * Column C starts at C x (0.75 x Width)
         * Row R starts at R x Height
         * A row's uneven columns have an vertical offset of 0.5 x Height 
         **/
        #region HexagonSideLength

        /// <summary>
        /// HexagonSideLength Dependency Property
        /// </summary>
        public static readonly DependencyProperty HexagonSideLengthProperty =
            DependencyProperty.Register("HexagonSideLength", typeof(double), typeof(HexagonGrid),
                new FrameworkPropertyMetadata((double)0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the HexagonSideLength property. This dependency property 
        /// represents the length of 1 side of the hexagon.
        /// </summary>
        public double HexagonSideLength
        {
            get { return (double)GetValue(HexagonSideLengthProperty); }
            set { SetValue(HexagonSideLengthProperty, value); }
        }

        #endregion

        #region Rows

        /// <summary>
        /// Rows Dependency Property
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(HexagonGrid),
                new FrameworkPropertyMetadata((int)1,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Rows property.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        #endregion

        #region Columns

        /// <summary>
        /// Columns Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(HexagonGrid),
                new FrameworkPropertyMetadata((int)1,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Columns property.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            var side = HexagonSideLength;
            var width = 2 * side;
            var height = side * Math.Sqrt(3.0);
            var colWidth = 0.75 * width;
            var rowHeight = height;

            var availableChildSize = new Size(width, height);

            foreach (FrameworkElement child in this.InternalChildren)
            {
                child.Measure(availableChildSize);
            }

            var totalHeight = Rows * rowHeight;
            if (Columns > 1)
                totalHeight += (0.5 * rowHeight);
            var totalWidth = Columns + (0.5 * side);

            var totalSize = new Size(totalWidth, totalHeight);

            return totalSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var side = HexagonSideLength;
            var width = 2 * side;
            var height = side * Math.Sqrt(3.0);
            var colWidth = 0.75 * width;
            var rowHeight = height;

            var childSize = new Size(width, height);

            foreach (FrameworkElement child in this.InternalChildren)
            {
                var row = GetRow(child);
                var col = GetColumn(child);

                var left = col * colWidth;
                var top = row * rowHeight;
                var isUnevenCol = (col % 2 != 0);
                if (isUnevenCol)
                    top += (0.5 * rowHeight);

                child.Arrange(new Rect(new Point(left, top), childSize));
            }

            return arrangeSize;
        }
    }
}
