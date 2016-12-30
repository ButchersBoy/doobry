using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Features
{
    public class FeatureRegistry
    {
        private readonly IDictionary<Guid, IFeatureFactory> _featureFactories;
        
        private FeatureRegistry(IDictionary<Guid, IFeatureFactory> featureFactories, IFeatureFactory defaultFeatureFactory)
        {
            _featureFactories = featureFactories;
        
            Default = defaultFeatureFactory;
        }

        public static FeatureRegistry WithDefault(IFeatureFactory featureFactory)
        {
            if (featureFactory == null) throw new ArgumentNullException(nameof(featureFactory));

            var featureFactories = new Dictionary<Guid, IFeatureFactory>
            {
                {featureFactory.FeatureId, featureFactory}
            };

            return new FeatureRegistry(featureFactories, featureFactory);
        }

        [Obsolete]
        public static FeatureRegistry WithDefault<TFeatureFactory>() where TFeatureFactory : IFeatureFactory, new()
        {
            var featureFactory = Activator.CreateInstance<TFeatureFactory>();
            var featureFactories = new Dictionary<Guid, IFeatureFactory>
            {
                {featureFactory.FeatureId, featureFactory}
            };

            return new FeatureRegistry(featureFactories, featureFactory);
        }

        public FeatureRegistry Add<TFeatureFactory>() where TFeatureFactory : IFeatureFactory, new()
        {
            var featureFactory = Activator.CreateInstance<TFeatureFactory>();
            if (_featureFactories.ContainsKey(featureFactory.FeatureId))
                throw new InvalidOperationException("A factory for the given " + nameof(featureFactory.FeatureId) + " already exists.");

            var featureFactories = new Dictionary<Guid, IFeatureFactory>(_featureFactories)
            {
                {featureFactory.FeatureId, featureFactory}
            };

            return new FeatureRegistry(featureFactories, Default);
        }

        public IFeatureFactory this[Guid featureId] => _featureFactories[featureId];

        public IFeatureFactory Default { get; }
    }
}
