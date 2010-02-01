using System;
using System.ComponentModel;

namespace Graffiti.Core
{

    public enum PostRenderLocation
    {
        Web = 0,
        Feed = 1,
        Other = 2
    }

    public class PostEventArgs : EventArgs
    {
        public PostEventArgs(Post post, PostRenderLocation renderLocation)
        {
            _post = post;
            _renderLocation = renderLocation;
        }

        private Post _post;
        public Post Post
        {
            get { return _post; }
        }

        private PostRenderLocation _renderLocation;
        public PostRenderLocation RenderLocation
        {
            get { return _renderLocation; }
        }

    }
}
