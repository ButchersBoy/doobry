using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Features
{
    public class FeatureRegistry
    {
        private readonly IDictionary<Guid, IFeatureFactory> _featureFactoriesById;
        private readonly IDictionary<Type, IFeatureFactory> _featureFactoriesByType;

        private FeatureRegistry(IDictionary<Guid, IFeatureFactory> featureFactoriesById, IDictionary<Type, IFeatureFactory> featureFactoriesByType, IFeatureFactory defaultFeatureFactory)
        {
            _featureFactoriesById = featureFactoriesById;
            _featureFactoriesByType = featureFactoriesByType;

            Default = defaultFeatureFactory;
        }

        public static FeatureRegistry WithDefault(IFeatureFactory featureFactory)
        {
            if (featureFactory == null) throw new ArgumentNullException(nameof(featureFactory));

            var featureFactoriesById = new Dictionary<Guid, IFeatureFactory>
            {
                {featureFactory.FeatureId, featureFactory}
            };
            var featureFactoriesByType = new Dictionary<Type, IFeatureFactory>
            {
                {featureFactory.GetType(), featureFactory}
            };

            return new FeatureRegistry(featureFactoriesById, featureFactoriesByType, featureFactory);
        }

        public FeatureRegistry Add(IFeatureFactory featureFactory)
        {
            if (featureFactory == null) throw new ArgumentNullException(nameof(featureFactory));
            if (_featureFactoriesById.ContainsKey(featureFactory.FeatureId))
                throw new InvalidOperationException("A factory for the given " + nameof(featureFactory.FeatureId) + " already exists.");

            var featureFactoryType = featureFactory.GetType();

            if (_featureFactoriesByType.ContainsKey(featureFactoryType))
                throw new InvalidOperationException("A factory for the given type " + featureFactoryType.FullName + " already exists.");

            var featureFactories = new Dictionary<Guid, IFeatureFactory>(_featureFactoriesById)
            {
                {featureFactory.FeatureId, featureFactory}
            };
            var featureFactoriesByType = new Dictionary<Type, IFeatureFactory>(_featureFactoriesByType)
            {
                {featureFactoryType, featureFactory}
            };

            return new FeatureRegistry(featureFactories, featureFactoriesByType, Default);
        }

        public IFeatureFactory this[Guid featureId] => _featureFactoriesById[featureId];

        public IFeatureFactory Get<TFeatureFactory>() where TFeatureFactory : IFeatureFactory
        {
            return _featureFactoriesByType[typeof(TFeatureFactory)];
        }

        public IFeatureFactory Default { get; }
    }
}
