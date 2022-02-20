using System;
using Xamarin.Forms;

namespace ConIView
{
	public class PinchToZoomContainer : ContentView
	{
		/// <summary>
		/// Hook into Tap, Pan and Pinch gestures for the view in this container
		/// </summary>
		public PinchToZoomContainer()
		{
			TapGestureRecognizer tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
			tap.Tapped += OnTapped;
			GestureRecognizers.Add( tap );

			PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
			pinchGesture.PinchUpdated += OnPinchUpdated;
			GestureRecognizers.Add( pinchGesture );

			PanGestureRecognizer pan = new PanGestureRecognizer();
			pan.PanUpdated += OnPanUpdated;
			GestureRecognizers.Add( pan );
		}

		/// <summary>
		/// Reset panning and zooming when the view size is set
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		protected override void OnSizeAllocated( double width, double height )
		{
			RestoreScaleValues();
			Content.AnchorX = 0.5;
			Content.AnchorY = 0.5;

			base.OnSizeAllocated( width, height );
		}

		/// <summary>
		/// Restore the zoom and starting position to defaults
		/// </summary>
		private void RestoreScaleValues()
		{
			_ = Content.ScaleTo( MinScale, 250, Easing.CubicInOut );
			_ = Content.TranslateTo( 0, 0, 250, Easing.CubicInOut );

			currentScale = MinScale;
			xOffset = Content.TranslationX = 0;
			yOffset = Content.TranslationY = 0;
		}

		/// <summary>
		/// When view is tapped either restore back to original size or scale to maximum size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTapped( object sender, EventArgs e )
		{
			if ( Content.Scale > MinScale )
			{
				RestoreScaleValues();
			}
			else
			{
				StartScaling();
				ExecuteScaling( MaxScale, .5, .5 );
				EndGesture();
			}
		}


		private void OnPanUpdated( object sender, PanUpdatedEventArgs e )
		{
			switch ( e.StatusType )
			{
				case GestureStatus.Started:
				{
					startX = e.TotalX;
					startY = e.TotalY;

					Content.AnchorX = 0;
					Content.AnchorY = 0;

					EnableSwipeInContainer( false );

					break;
				}
				case GestureStatus.Running:
				{
					double maxTranslationX = ( Content.Scale * Content.Width ) - Content.Width;
					Content.TranslationX = Math.Min( 0, Math.Max( -maxTranslationX, xOffset + e.TotalX - startX ) );

					double maxTranslationY = ( Content.Scale * Content.Height ) - Content.Height;
					Content.TranslationY = Math.Min( 0, Math.Max( -maxTranslationY, yOffset + e.TotalY - startY ) );

					break;
				}
				case GestureStatus.Completed:
				{
					EndGesture();
					EnableSwipeInContainer( true );

					break;
				}

				default:
				{
					break;
				}
			}
		}

		private void OnPinchUpdated( object sender, PinchGestureUpdatedEventArgs e )
		{
			if ( e.Status == GestureStatus.Started )
			{
				StartScaling();

				EnableSwipeInContainer( false );
			}
			if ( e.Status == GestureStatus.Running )
			{
				ExecuteScaling( e.Scale, e.ScaleOrigin.X, e.ScaleOrigin.Y );
			}
			if ( e.Status == GestureStatus.Completed )
			{
				EndGesture();

				EnableSwipeInContainer( true );
			}
		}

		private void StartScaling()
		{
			// Store the current scale factor applied to the wrapped user interface element,
			// and zero the components for the center point of the translate transform.
			startScale = Content.Scale;

			Content.AnchorX = 0;
			Content.AnchorY = 0;
		}

		private void ExecuteScaling( double scale, double x, double y )
		{
			currentScale += ( scale - 1 ) * startScale;
			currentScale = Math.Max( MinScale, currentScale );
			currentScale = Math.Min( MaxScale, currentScale );

			// Calculate the scale factor to be applied.
			double deltaX = ( Content.X + xOffset ) / Width;
			double deltaWidth = Width / ( Content.Width * startScale );
			double originX = ( x - deltaX ) * deltaWidth;

			// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
			// so get the X pixel coordinate.
			double deltaY = ( Content.Y + yOffset ) / Height;
			double deltaHeight = Height / ( Content.Height * startScale );
			double originY = ( y - deltaY ) * deltaHeight;

			// Calculate the transformed element pixel coordinates.
			double targetX = xOffset - ( originX * Content.Width * ( currentScale - startScale ) );
			double targetY = yOffset - ( originY * Content.Height * ( currentScale - startScale ) );

			// Apply translation based on the change in origin.
			Content.TranslationX = targetX.Clamp( -Content.Width * ( currentScale - 1 ), 0 );
			Content.TranslationY = targetY.Clamp( -Content.Height * ( currentScale - 1 ), 0 );

			// Apply scale factor.
			Content.Scale = currentScale;
		}

		private void EndGesture()
		{
			// Store the translation delta's of the wrapped user interface element.
			xOffset = Content.TranslationX;
			yOffset = Content.TranslationY;
		}

		/// <summary>
		/// Enable or disable swipe detection in the parent carousel view.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableSwipeInContainer( bool enable )
		{
			// Has the parent been found yet
			if ( carouselView == null )
			{
				Element parent = Parent;
				while ( ( parent != null ) && ( ( parent is CarouselView ) == false ) )
				{
					parent = parent.Parent;
				}

				carouselView = parent as CarouselView;
			}

			if ( carouselView != null )
			{
				carouselView.IsSwipeEnabled = enable;
			}
		}

		public double MinScale { get; set; } = 1;

		public double MaxScale { get; set; } = 3;

		private double currentScale = 1;
		private double startScale = 1;
		private double xOffset = 0;
		private double yOffset = 0;
		private double startX = 0;
		private double startY = 0;

		private CarouselView carouselView = null;
	}

	public static class DoubleExtensions
	{
		public static double Clamp( this double self, double min, double max ) => Math.Min( max, Math.Max( self, min ) );
	}
}
