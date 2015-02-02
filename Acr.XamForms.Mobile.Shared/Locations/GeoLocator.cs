using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acr.XamForms.Mobile.Locations {
    
    public class GeoLocator : IGeoLocator {

        private readonly GeoLocator locator;

        public GeoLocator() {
            this.locator = new GeoLocator();
            this.locator.PositionChanged += this.OnPositionChanged;
            this.locator.PositionError += this.OnPositionError;
        }

        #region Internals

        private void OnPositionChanged(object sender, PositionEventArgs e) {
            if (this.PositionChanged != null) { 
                var pos = ToFormsPosition(e.Position);
                this.PositionChanged(this, new PositionEventArgs(pos));
            }
        }


        private void OnPositionError(object sender, PositionErrorEventArgs e) {
            if (this.PositionError != null) {
				var error = e.Error == GeoLocationError.Unauthorized
                    ? GeoLocationError.Unauthorized
                    : GeoLocationError.PositionUnavailable;
                this.PositionError(this, new PositionErrorEventArgs(error));
            }
        }


        private static Position ToFormsPosition(Position pos) {
            return new Position {
                Accuracy = pos.Accuracy,
                Altitude = pos.Altitude,
                AltitudeAccuracy = pos.AltitudeAccuracy,
                Heading = pos.Heading,
                Latitude = pos.Latitude,
                Longitude = pos.Longitude,
                Speed = pos.Speed,
                Timestamp = pos.Timestamp
            };
        }

        #endregion

        #region IGeoLocator Members

        public double DesiredAccuracy {
            get { return this.locator.DesiredAccuracy; }
            set { this.locator.DesiredAccuracy = value; }
        }


        public bool IsListening {
            get { return this.locator.IsListening; }
        }


        public bool IsGeoLocationAvailable {
			get { return this.locator.IsGeoLocationAvailable; }
        }


        public bool SupportsHeading {
            get { return this.locator.SupportsHeading; }
        }


        public void StartListening(int minTime, double minDistance, bool includeHeading = false) {
            this.locator.StartListening(minTime, minDistance, includeHeading);
        }


        public void StopListening() {
            this.locator.StopListening();
        }


        public async Task<Position> GetPositionAsync(int timeout, bool includeHeading, CancellationToken cancelToken) {
            var turnBackOff = false;
            if (!this.IsListening) {
                turnBackOff = true;
                this.StartListening(1, 10);
            }
			var pos = await this.locator.GetPositionAsync(timeout, includeHeading, cancelToken);
            if (turnBackOff)
                this.StopListening();

            return ToFormsPosition(pos);
        }

        public event EventHandler<PositionEventArgs> PositionChanged;

        public event EventHandler<PositionErrorEventArgs> PositionError;

        #endregion
    }
}