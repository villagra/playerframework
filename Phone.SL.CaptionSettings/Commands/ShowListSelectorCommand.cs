namespace Microsoft.PlayerFramework.CaptionSettings.Commands
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;

    public class ShowListSelectorCommand : ICommand
    {
        PhoneApplicationPage page;
        IList itemsSource;
        string title;
        LongListSelector listSelector;
        string templateName;

        public ShowListSelectorCommand(
            PhoneApplicationPage page,
            string title,
            IList itemsSource,
            LongListSelector listSelector,
            string templateName)
        {
            this.page = page;
            this.title = title;
            this.itemsSource = itemsSource;
            this.listSelector = listSelector;
            this.templateName = templateName;
        }

        public bool CanExecute(object parameter)
        {
            var viewModel = parameter as CaptionSettingsFlyoutViewModel;

            if (!viewModel.IsEnabled)
            {
                return false;
            }



            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var viewModel = parameter as CaptionSettingsFlyoutViewModel;

            this.ShowListSelector(viewModel);
        }

        #region Implementation
        private void ShowListSelector(CaptionSettingsFlyoutViewModel viewModel)
        {
            //this.PageTitle.Text = this.title;
            this.listSelector.ItemsSource = this.itemsSource;
            //this.listSelector.SelectedItem = selectedItem;
            this.listSelector.SelectionChanged += this.OnSelectionChanged;
            this.listSelector.ItemTemplate = this.page.Resources[this.templateName] as DataTemplate;

            VisualStateManager.GoToState(this.page, "ListShown", true);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listSelector.SelectionChanged -= this.OnSelectionChanged;
        }
        #endregion

    }
}
