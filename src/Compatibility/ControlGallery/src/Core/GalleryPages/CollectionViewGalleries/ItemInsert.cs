using System;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.GalleryPages.CollectionViewGalleries
{
	internal class ItemInserter : ObservableCollectionModifier
	{
		public ItemInserter(CollectionView cv) : base(cv, "Insert")
		{
		}

		protected override void ModifyObservableCollection(ObservableCollection<CollectionViewGalleryTestItem> observableCollection, params int[] indexes)
		{
			var index = indexes[0];

			if (index > -1 && index <= observableCollection.Count)
			{
				var item = new CollectionViewGalleryTestItem(DateTime.Now, "Inserted", "oasis.jpg", index);
				observableCollection.Insert(index, item);
			}
		}
	}
}

