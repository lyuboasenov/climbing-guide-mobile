using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Views.CustomViews {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Breadcrumb : ContentView {
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(Breadcrumb), null,
            propertyChanged: ItemsSourceChanged);

      public IEnumerable ItemsSource {
         get { return (IEnumerable)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }

      private StackLayout BreadcrumbStackLayout {
         get { return breadcrumbStackLayout; }
         set { breadcrumbStackLayout = value; }
      }

      private ScrollView BreadCrumbsScrollView {
         get { return breadCrumbsScrollView; }
         set { breadCrumbsScrollView = value; }
      }

      private Label DisplayMember { get; }

      private BindingBase itemDisplayBinding;
      public BindingBase ItemDisplayBinding {
         get { return itemDisplayBinding; }
         set {
            if (itemDisplayBinding == value)
               return;

            OnPropertyChanging();
            var oldValue = value;
            itemDisplayBinding = value;
            OnItemDisplayBindingChanged(oldValue, itemDisplayBinding);
            OnPropertyChanged();
         }
      }

      public Breadcrumb() {
         InitializeComponent();
         DisplayMember = new Label();
      }

      private static void ItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue) {
         var control = (Breadcrumb)bindable;

         var oldNotifyPropertyChanged = oldvalue as INotifyCollectionChanged;
         if (null != oldNotifyPropertyChanged) {
            oldNotifyPropertyChanged.CollectionChanged -= control.ItemsSourceCollectionChanged;
         }

         var newNotifyPropertyChanged = newvalue as INotifyCollectionChanged;
         if (null != newNotifyPropertyChanged) {
            newNotifyPropertyChanged.CollectionChanged += control.ItemsSourceCollectionChanged;
         }

         control.ResetItems();
      }

      private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
         ResetItems();
      }

      private void OnItemDisplayBindingChanged(BindingBase oldValue, BindingBase newValue) {
         ResetItems();
      }

      private void AnimatedStack_ChildAdded(object sender, ElementEventArgs e) {
         Device.BeginInvokeOnMainThread(() => {
            var width = Application.Current.MainPage.Width;

            var storyboard = new Animation();
            var enterRight = new Animation(callback: d =>
            BreadcrumbStackLayout.Children.Last().TranslationX = d,
            start: width,
            end: 0,
            easing: Easing.Linear);

            storyboard.Add(0, 1, enterRight);
            storyboard.Commit(BreadcrumbStackLayout.Children.Last(),
                "RightToLeftAnimation", length: 800);
         });
      }

      private void ResetItems() {
         BreadcrumbStackLayout.Children.Clear();
         if (null != ItemsSource) {
            foreach (var item in ItemsSource) {
               if (null != item) {
                  AddItem(GetDisplayMember(item));
               }
            }
         }
      }

      private string GetDisplayMember(object item) {
         if (null == item) {
            throw new ArgumentNullException(nameof(item));
         }

         if (ItemDisplayBinding == null)
            return item.ToString();

         DisplayMember.BindingContext = item;
         DisplayMember.SetBinding(Label.TextProperty, ItemDisplayBinding);
         var result = DisplayMember.Text;
         DisplayMember.RemoveBinding(Label.TextProperty);

         return (string)result;
      }

      private void AddItem(string itemName) {
         // Add the new Breadcrumb Label
         BreadcrumbStackLayout.Children.Add(new Label {
            Text = $"/ {itemName}",
            FontSize = 15,
            TextColor = Color.Black,
         });

         // Scroll to the end of the StackLayout
         BreadCrumbsScrollView.ScrollToAsync(BreadcrumbStackLayout,
            ScrollToPosition.End, true);
      }
   }
}