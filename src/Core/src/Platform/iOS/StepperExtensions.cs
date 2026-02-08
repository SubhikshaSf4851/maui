using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class StepperExtensions
	{
		public static void UpdateMinimum(this UIStepper platformStepper, IStepper stepper)
		{
			platformStepper.MinimumValue = stepper.Minimum;
		}

		public static void UpdateMaximum(this UIStepper platformStepper, IStepper stepper)
		{
			platformStepper.MaximumValue = stepper.Maximum;
		}

		public static void UpdateIncrement(this UIStepper platformStepper, IStepper stepper)
		{
			var increment = stepper.Interval;

			if (increment > 0)
				platformStepper.StepValue = stepper.Interval;
		}

		public static void UpdateValue(this UIStepper platformStepper, IStepper stepper)
		{
			// Update MinimumValue first to prevent UIStepper from incorrectly clamping the Value.
			// If MAUI updates Value before Minimum, a stale higher MinimumValue would cause iOS to clamp Value incorrectly.
			if (platformStepper.MinimumValue != stepper.Minimum)
			{
				platformStepper.MinimumValue = stepper.Minimum;
			}

			if (platformStepper.Value != stepper.Value)
			{
				platformStepper.Value = stepper.Value;
			}

		}

		internal static void UpdateFlowDirection(this UIStepper platformStepper, IStepper stepper)
		{
			// Determine the effective FlowDirection (resolve MatchParent)
			CoreGraphics.CGAffineTransform transform;

			if (stepper.FlowDirection == FlowDirection.RightToLeft)
			{
				// Explicit RTL
				transform = GetRTLTransform();
			}
			else if (stepper.FlowDirection == FlowDirection.LeftToRight)
			{
				// Explicit LTR
				transform = CoreGraphics.CGAffineTransform.MakeIdentity();
			}
			else // FlowDirection.MatchParent
			{
				// Check parent's direction
				var parent = (stepper as IView)?.Parent?.Handler?.PlatformView as UIView;
				if (parent != null && parent.SemanticContentAttribute == UISemanticContentAttribute.ForceRightToLeft)
				{
					// Parent is RTL, so inherit RTL
					transform = GetRTLTransform();
				}
				else
				{
					// Parent is LTR or unspecified, default to LTR
					transform = CoreGraphics.CGAffineTransform.MakeIdentity();
				}
			}

			// Apply transform to the stepper and its subviews
			platformStepper.Transform = transform;
			foreach (var subview in platformStepper.Subviews)
			{
				subview.Transform = transform;
			}
		}

		static CoreGraphics.CGAffineTransform GetRTLTransform()
		{
			// Flip horizontally for RTL
			return CoreGraphics.CGAffineTransform.MakeScale(-1, 1);
		}
	}
}