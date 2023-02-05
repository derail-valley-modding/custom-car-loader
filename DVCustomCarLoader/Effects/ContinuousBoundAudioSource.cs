namespace DVCustomCarLoader.Effects
{
    public class ContinuousBoundAudioSource : BoundAudioSource
    {
        public void Awake()
        {
            Initialize();

            Source.loop = true;
        }

        public void Start()
        {
            Source.Play();
        }
    }
}