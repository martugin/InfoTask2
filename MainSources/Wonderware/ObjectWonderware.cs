using CommonTypes;

namespace Provider
{
    //Объект
    internal class ObjectWonderware : SourceObject
    {
        public ObjectWonderware(string tag) : base(tag)
        {
            Inf = TagName = tag;
        }

        //Имя тэга 
        public string TagName { get; private set; }
    }
}