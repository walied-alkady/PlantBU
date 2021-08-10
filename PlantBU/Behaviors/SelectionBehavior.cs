// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-14-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 05-14-2021
// ***********************************************************************
// <copyright file="SelectionBehavior.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PlantBU.Behaviors
{
    
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}'
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}'
#pragma warning disable CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning disable CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
/// <summary>
    /// Class SelectionBehavior.
    /// Implements the <see cref="Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}" />
    [Preserve(AllMembers = true)]
#pragma warning restore CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning restore CS1658 // Type parameter declaration must be an identifier not a type. See also error CS0081.
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}'
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute 'Xamarin.Forms.Behavior{Syncfusion.XForms.Buttons.SfSegmentedControl}'
    public class SelectionBehavior : Behavior<Syncfusion.XForms.Buttons.SfSegmentedControl>
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(SelectionBehavior), null);

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(SelectionBehavior), null);

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
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        public Syncfusion.XForms.Buttons.SfSegmentedControl AssociatedObject { get; private set; }

        /// <summary>
        /// Attaches to the superclass and then calls the <see cref="M:Xamarin.Forms.Behavior`1.OnAttachedTo(`0)" /> method on this object.
        /// </summary>
        /// <param name="bindable">The bindable object to which the behavior was attached.</param>
        /// <remarks>To be added.</remarks>
        protected override void OnAttachedTo(Syncfusion.XForms.Buttons.SfSegmentedControl bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.SelectionChanged += SelectionChanged;
        }

        /// <summary>
        /// Calls the <see cref="M:Xamarin.Forms.Behavior`1.OnDetachingFrom(`0)" /> method and then detaches from the superclass.
        /// </summary>
        /// <param name="bindable">The bindable object from which the behavior was detached.</param>
        /// <remarks>To be added.</remarks>
        protected override void OnDetachingFrom(Syncfusion.XForms.Buttons.SfSegmentedControl bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.SelectionChanged -= SelectionChanged;
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
        /// Selections the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.XForms.Buttons.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SelectionChanged(object sender, Syncfusion.XForms.Buttons.SelectionChangedEventArgs e)
        {
            if (Command == null)
            {
                return;
            }

            var paramerter = new List<object> { e, CommandParameter };
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
