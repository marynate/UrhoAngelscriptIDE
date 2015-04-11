using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Debugger.IDE.Intellisense {

    public class BaseCompletionData : ICompletionData {
        protected string text_;
        protected string desc_;
        protected string img_;
        protected BitmapImage img;
        static Dictionary<string, BitmapImage> sourcePool = new Dictionary<string, BitmapImage>();
        protected double priority_ = 1;

        public BaseCompletionData(string img, string str, string desc = null) {
            text_ = str;
            img_ = img;
            desc_ = desc;
        }

        public virtual void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            textArea.Document.Replace(completionSegment, text_);
        }

        public virtual object Content {
            get { return text_; }
        }

        public object Description {
            get { return desc_; }
        }

        public System.Windows.Media.ImageSource Image {
            get {
                if (img == null && img_ != null) {
                    //first appearance
                    if (sourcePool.ContainsKey(img_)) {
                        img = sourcePool[img_];
                    } else {
                        //repeated appearance
                        BitmapImage bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(string.Format("pack://application:,,,/Debugger;component/Images/all/{0}", img_), UriKind.Absolute);
                        bmp.EndInit();
                        img = bmp;
                        sourcePool[img_] = bmp;
                    }
                }
                return img;
            }
        }

        public double Priority {
            get { return priority_; }
            set { priority_ = value; }
        }

        public string Text {
            get { return text_; }
        }
    }

    public class ClassCompletionData : BaseCompletionData {
        TypeInfo classType_;

        public ClassCompletionData(TypeInfo aType) : base("class.png", aType.Name, "") {
            classType_ = aType;
            if (aType is EnumInfo)
                img_ = "enum.png";
        }
    }

    public class PropertyCompletionData : BaseCompletionData {
        TypeInfo classType_;

        public PropertyCompletionData(TypeInfo aType, string desc, bool aReadOnly = false) : base(aReadOnly ? "roproperty.png" : "property.png", desc, aType.Name) {
            classType_ = aType;
        }
    }

    public class FunctionCompletionData : BaseCompletionData {
        FunctionInfo func_;

        public FunctionCompletionData(FunctionInfo aFunc)
            : base("method.png", string.Format("{0}{1}", aFunc.Name, aFunc.Inner), aFunc.ReturnType.Name) {
            func_ = aFunc;
        }

        public override void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            if (func_.Inner != null && func_.Inner.Length > 2)
                textArea.Document.Replace(completionSegment, func_.Name.Trim() + "(");
            else
                textArea.Document.Replace(completionSegment, text_);
        }
    }
}
