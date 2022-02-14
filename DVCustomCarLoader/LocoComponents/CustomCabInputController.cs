using System.Collections;
using System.Linq;

namespace DVCustomCarLoader.LocoComponents
{
    public class CustomCabInputController : CabInput
    {
		protected ICabControlAcceptor[] controlAcceptors;
		public CabInputRelay[] Relays;

		protected virtual void Start()
		{
			var car = TrainCar.Resolve(gameObject);
			if( car == null || !car )
			{
				Main.Error($"Couldn't find TrainCar for interior {gameObject.name}");
				return;
			}

			controlAcceptors = 
				car.gameObject.GetComponentsByInterface<ICabControlAcceptor>()
				.Concat(gameObject.GetComponentsByInterface<ICabControlAcceptor>())
				.ToArray();

			if( controlAcceptors.Length == 0 )
			{
				Main.Error("Couldn't find any components accepting cab input");
				return;
			}

			Relays = GetComponentsInChildren<CabInputRelay>(true);
			Main.LogVerbose($"CustomCabInput Start - {Relays.Length} controls");
			foreach( CabInputRelay relay in Relays )
			{
				foreach( var receiver in controlAcceptors )
                {
					if( receiver.AcceptsControlOfType(relay.Binding) )
					{
						Main.LogVerbose($"Add {relay.GetType().Name} {relay.name} to {receiver.GetType().Name}");
						receiver.RegisterControl(relay);
					}
                }
			}
		}
	}
}