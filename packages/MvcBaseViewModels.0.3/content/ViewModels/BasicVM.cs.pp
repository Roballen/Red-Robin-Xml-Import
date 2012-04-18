using System.Collections.Generic;

namespace $rootnamespace$.ViewModels
{
    public class BasicVM<T> : BaseViewModel where T : new()
    {
        public T Data { get; set; }
        public IList<T> DataList { get; set; }
    }
}