using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Utilities
{
    [ContentProperty(nameof(Source))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension'
    public class ImageResourceExtension : IMarkupExtension
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension.Source'
        public string Source { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension.Source'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension.ProvideValue(IServiceProvider)'
        public object ProvideValue(IServiceProvider serviceProvider)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ImageResourceExtension.ProvideValue(IServiceProvider)'
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            return imageSource;
        }
    }
}
