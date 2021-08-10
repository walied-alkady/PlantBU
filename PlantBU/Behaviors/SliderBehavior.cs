// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-14-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 05-14-2021
// ***********************************************************************
// <copyright file="SliderBehavior.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PlantBU.Behaviors
{
    
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Xamarin.Forms.Slider}'
#pragma warning disable CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Xamarin.Forms.Slider}'
#pragma warning disable CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
/// <summary>
    /// Class SliderBehavior.
    /// Implements the <see cref="Xamarin.Forms.Behavior{Xamarin.Forms.Slider}" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Behavior{Xamarin.Forms.Slider}" />
    [Preserve(AllMembers = true)]
#pragma warning restore CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Xamarin.Forms.Slider}'
#pragma warning restore CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Xamarin.Forms.Slider}'
    public class SliderBehavior : Behavior<Slider>
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(SliderBehavior), null);

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        public Slider AssociatedObject { get; private set; }

        /// <summary>
        /// Attaches to the superclass and then calls the <see cref="M:Xamarin.Forms.Behavior`1.OnAttachedTo(`0)" /> method on this object.
        /// </summary>
        /// <param name="bindable">The bindable object to which the behavior was attached.</param>
        /// <remarks>To be added.</remarks>
        protected override void OnAttachedTo(Slider bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.ValueChanged += ValueChanged;
        }

        /// <summary>
        /// Calls the <see cref="M:Xamarin.Forms.Behavior`1.OnDetachingFrom(`0)" /> method and then detaches from the superclass.
        /// </summary>
        /// <param name="bindable">The bindable object from which the behavior was detached.</param>
        /// <remarks>To be added.</remarks>
        protected override void OnDetachingFrom(Slider bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.ValueChanged -= ValueChanged;
            AssociatedObject = null;
        }

        /// <summary>
        /// Override this method to execute an action when the BindingContext changes.
        /// </summary>
        /// <remarks>To be added.</remarks>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        /// <summary>
        /// Values the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
        private void ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Command == null)
            {
                return;
            }

            var paramerter = e;
            if (Command.CanExecute(paramerter))
            {
                Command.Execute(paramerter);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:BindingContextChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }
    }
}
