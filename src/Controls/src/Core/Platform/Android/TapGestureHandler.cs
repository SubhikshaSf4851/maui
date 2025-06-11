using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Views;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Platform
{
	internal class TapGestureHandler
	{
		int _taps;
		DateTime _tapTime;
		int _numberOfTapsRequired;
		public TapGestureHandler(Func<View?> getView, Func<IList<GestureElement>> getChildElements)
		{
			GetView = getView;
			GetChildElements = getChildElements;
		}

		Func<IList<GestureElement>> GetChildElements { get; }
		Func<View?> GetView { get; }

		public void OnSingleClick()
		{
			// only handle click if we don't have double tap registered
			if (TapGestureRecognizers(2).Any())
				return;

			OnTap(1, null);
		}

		public bool OnTap(int count, MotionEvent? e)
		{
			Point point;

			if (e == null)
				point = new Point(-1, -1);
			else
				point = new Point(e.GetX(), e.GetY());

			var view = GetView();

			if (view == null)
				return false;
			if (_taps == 0)
			{
				_tapTime = DateTime.Now;
			}
			_taps++;  //track no of taps done by user.

			var captured = false;

			var children = view.GetChildElements(point);

			if (children != null)
			{
				foreach (var recognizer in children.GetChildGesturesFor<TapGestureRecognizer>(recognizer => recognizer.NumberOfTapsRequired == count))
				{
					if (!CheckButtonMask(recognizer, e))
						continue;

					recognizer.SendTapped(view, (view) => e.CalculatePosition(GetView(), view));
					captured = true;
				}
			}

			if (captured)
				return captured;

			IEnumerable<TapGestureRecognizer> tapGestures = view.GestureRecognizers.GetGesturesFor<TapGestureRecognizer>();
			GetNumberOfTapsRequired(tapGestures.FirstOrDefault() ?? new TapGestureRecognizer());
			// Debug.WriteLine("_numberOfTapsRequired: " + _numberOfTapsRequired);
			if (_numberOfTapsRequired < 2)
			{
				Debug.WriteLine("TapGestureHandler count value: " + count);
				TriggerGestures(count);
			}
			else
			{
				Debug.WriteLine((DateTime.Now < _tapTime.AddMilliseconds(1000)) + "tapTime");
				if (_taps > 0 && DateTime.Now < _tapTime.AddMilliseconds(1000))
				{
					if (_taps == _numberOfTapsRequired)
					{
						TriggerGestures(_taps);
						_taps = 0; // Reset taps after processing
					}
					else
					{
						Debug.WriteLine(_taps + "tapTime");
						_tapTime = DateTime.Now;
					}
				}
				else
				{
					Debug.WriteLine("Else Executed, Tap becomes zero");
					_taps = 0;
				}
			}

			void TriggerGestures(int count)
			{
				IEnumerable<TapGestureRecognizer> gestureRecognizers = Enumerable.Empty<TapGestureRecognizer>();
				gestureRecognizers = TapGestureRecognizers(count);
				foreach (var gestureRecognizer in gestureRecognizers)
				{
					if (!CheckButtonMask(gestureRecognizer, e))
						continue;

					gestureRecognizer.SendTapped(view, (view) => e.CalculatePosition(GetView(), view));
					Debug.WriteLine("Tapped triggered ");
					captured = true;
				}
			}

			return captured;

			bool CheckButtonMask(TapGestureRecognizer tapGestureRecognizer, MotionEvent? motionEvent)
			{
				if (tapGestureRecognizer.Buttons == ButtonsMask.Secondary)
				{
					var buttonState = motionEvent?.ButtonState ?? MotionEventButtonState.Primary;

					return
						buttonState == MotionEventButtonState.Secondary ||
						buttonState == MotionEventButtonState.StylusSecondary;
				}

				return (tapGestureRecognizer.Buttons & ButtonsMask.Primary) == ButtonsMask.Primary;
			}

			void GetNumberOfTapsRequired(TapGestureRecognizer g)
			{
				_numberOfTapsRequired = g.NumberOfTapsRequired;
			}
		}

		public bool HasAnyGestures()
		{
			var view = GetView();
			return view != null && view.GestureRecognizers.OfType<TapGestureRecognizer>().Any()
								|| GetChildElements().GetChildGesturesFor<TapGestureRecognizer>().Any();
		}

		public IEnumerable<TapGestureRecognizer> TapGestureRecognizers(int count)
		{
			var view = GetView();
			if (view == null)
				return Enumerable.Empty<TapGestureRecognizer>();

			return view.GestureRecognizers.GetGesturesFor<TapGestureRecognizer>(recognizer => recognizer.NumberOfTapsRequired == count);
		}

	}
}