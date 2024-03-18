namespace UniRx
{
    using System;
    public static class UIExtensionUniRx
    {
        public static IDisposable AddButtonClick(this UnityEngine.UI.Button button, Action onClick, UnityEngine.Component component, float skip = 0.25f)
        {
            return button.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(skip), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe(_ => { onClick?.Invoke(); }).AddTo(component);
        }

        public static IDisposable SubscribeToTextMeshPro<T>(this IObservable<T> source, TMPro.TMP_Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }

        public static IDisposable SubscribeToTextMeshPro<T>(this IObservable<T> source, TMPro.TMP_Text text, Func<T, string> selector)
        {
            return source.SubscribeWithState2(text, selector, (x, t, s) => t.text = s(x));
        }

        public static IDisposable SubscribeToTextMeshPro<T>(this IObservable<T> source, TMPro.TMP_Text text,string format)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = string.Format(format, x.ToString()));
        }

        public static IDisposable SubscribeToTextMeshProFormat<T>(this IObservable<T> source, TMPro.TMP_Text text, Func<T,string> formatFunc)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = formatFunc(x));
        }
    }
}
