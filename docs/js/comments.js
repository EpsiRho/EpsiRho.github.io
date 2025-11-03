document.addEventListener("DOMContentLoaded", () => {
  const blueskyHandle = "epsirho.com";
  const commentTag = "blog";
  let did = "";
  const commentsContainer = document.getElementById("comments-section");
  let ConnectPostUrl = '';
  async function fetchCommentsForPost(postUri) {
    const apiUrl =
      "https://api.bsky.app/xrpc/app.bsky.feed.getPostThread";
    const params = new URLSearchParams({ uri: postUri });

    try {
      // Fetch the post thread from the Bluesky API
      const response = await fetch(`${apiUrl}?${params.toString()}`);

      // Check if the response is successful
      if (!response.ok) {
        throw new Error(`Error fetching post thread: ${response.statusText}`);
      }

      // Parse the JSON response
      const data = await response.json();

      // Extract the thread and build a tree structure
      const thread = data.thread;
      //const tree = buildTree(thread);
      const tree = thread.replies.map((reply) => buildTree(reply));

      return tree;
    } catch (error) {
      console.error("Failed to fetch comments:", error);
      return {};
    }
  }

  function buildTree(node) {
    // Extract the handle and rkey
    const parts = node.post?.uri.split("/");
    const rkey = parts[4]; // xyz123

    // Construct the URL
    const postUrl = `https://bsky.app/profile/${node.post?.author.handle}/post/${rkey}`;

    const dt = new Date(node.post?.record.createdAt);

    const formattedDate = dt.toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });

    let cleanHTML = DOMPurify.sanitize(node.post?.record.text, {
        ALLOWED_TAGS: ['b', 'i', 'em', 'strong', 'p', 'span']
      });
    
    if(cleanHTML != node.post?.record.text){
        cleanHTML += ` <span title="This user posted content that needed to be filtered out for saftey, likely html/css code that could be used for injection." style="color: red;">*</span>`;
        console.log('Comment Sanatized!');
    }

    let img = document.createElement('div');

    if(node.post?.record.embed && node.post?.record.embed.images){
        img.classList.add('comment-images-holder')
        node.post?.record.embed.images.forEach(item => {
            let tempDiv = document.createElement(`div`);
            tempDiv.classList.add(`comment-image-container`);
            let tempImg = document.createElement(`img`);
            tempImg.src = `https://cdn.bsky.app/img/feed_fullsize/plain/${node.post?.author.did}/${item.image.ref.$link}@png`
            tempImg.classList.add('comment-image');
            tempDiv.appendChild(tempImg);
            img.appendChild(tempDiv);
            
        });
    }
    
    const treeNode = {
      id: node.post?.uri || null,
      author: node.post?.author.handle || "Unknown",
      authorpfp: node.post?.author.avatar || "Unknown",
      authorDisplay: node.post?.author.displayName || "Unknown",
      createdAt: formattedDate || "Unknown",
      likeCount: node.post?.likeCount || 0,
      repostTotal: node.post?.quoteCount || 0 + node.post?.repostCount || 0,
      webLink: postUrl,
      imageEmbed: img,
      text: cleanHTML || "",
      children: [],
    };

    // Recursively build children for each reply
    if (node.replies && node.replies.length > 0) {
      for (const reply of node.replies) {
        if (reply.post) {
          // Recursively build the tree for this reply
          const childNode = buildTree(reply);
          treeNode.children.push(childNode);
        }
      }
    }

    return treeNode;
  }

  function renderComments(nodes, container, isChild) {
    nodes.forEach((node) => {
      // Create the comment element
      const commentElement = document.createElement("div");
      commentElement.classList.add("comment");
      let OP = "";
      if (isChild) {
        commentElement.classList.add(`reply`);
      }
      if (node.author == "epsirho.com") {
        OP = `✨`;
      }

      commentElement.innerHTML = `
            <a class="externalLink" title="View comment on bluesky" href="${node.webLink}">&#x2924</a>
            <div class="author-info">
                <div class="authorpfp"><img src="${node.authorpfp}"></img></div>
                <div class="authorNameInfo">
                    <div class="author-display ${OP}">${node.authorDisplay} ${OP}</div>
                    <div class="author">@${node.author}</div>
                </div>
            </div>
            <div class="text">${node.text}</div>
            ${node.imageEmbed.outerHTML}
            <div>
                <div class="date">${node.createdAt}  &#x2022;  &#9829; ${node.likeCount}  &#x2022;  &#x1F5D8; ${node.repostTotal}</div>
            </div>
        `;

      // Append the comment to the container
      container.appendChild(commentElement);

      // If there are children, render them recursively
      if (node.children && node.children.length > 0) {
        const repliesContainer = document.createElement("div");
        repliesContainer.classList.add("replies");
        commentElement.appendChild(repliesContainer);

        renderComments(node.children, repliesContainer, true);
      }
    });
  }

  async function fetchUserPostsByHashtag(userHandle, hashtag) {
    const apiUrl = "https://api.bsky.app/xrpc/app.bsky.feed.searchPosts";
    const currentFullUrl = window.location.href;
    const currentRelativePath = window.location.pathname;
    let allPosts = [];
    let cursor = null;
    let hasMore = true;

    while (hasMore) {
      try {
        // Fetch posts from the Bluesky API
        const params = new URLSearchParams({
          q: `#${hashtag} from:${userHandle}`,
          limit: "50", 
          ...(cursor && { cursor }), 
        });
        const response = await fetch(`${apiUrl}?${params.toString()}`);

        if (!response.ok) {
          throw new Error(`Error fetching posts: ${response.statusText}`);
        }

        const data = await response.json();

        data.posts.forEach((post) => {
          if(did == ''){
            did = post.author.did;
          }
          let url = post.embed.external.uri;
          if (url.includes(currentRelativePath)) {
            const parts = post.uri.split("/");
            const rkey = parts[4]; 
            ConnectPostUrl = `https://bsky.app/profile/${userHandle}/post/${rkey}`;
            fetchCommentsForPost(post.uri)
              .then((comments) => {
                renderComments(comments, commentsContainer, false);
              })
              .catch((error) => {
                console.error("Error:", error);
              });

            return true;
          } else {
          }
        });

        // Pagination
        cursor = data.cursor;

        // Are there more posts to fetch?
        hasMore = !!cursor;
      } catch (error) {
        console.error("Failed to fetch posts:", error);
        break;
      }
    }

    return false;
  }

  const loadingBarContainer = document.getElementById("loading-bar-container");

  loadingBarContainer.style.display = "block";

  fetchUserPostsByHashtag(blueskyHandle, commentTag)
    .then((posts) => {
      console.log(`Found comment post: ${ConnectPostUrl}`);

      if(ConnectPostUrl == ''){
        const metaElement = document.querySelector('[bsky-comments-link]');
        const commentsLink = metaElement.getAttribute('bsky-comments-link');

        if(commentsLink !== ''){
          ConnectPostUrl = `https://bsky.app/profile/${blueskyHandle}/post/${commentsLink}`;
          const uri = `at://${did}/app.bsky.feed.post/${commentsLink}`;
          fetchCommentsForPost(uri)
              .then((comments) => {
                renderComments(comments, commentsContainer, false);
              })
              .catch((error) => {
                console.error("Error:", error);
              });
        } 
      }

      if (ConnectPostUrl == '') {
        const noCommentsInfo = document.createElement("div");

        noCommentsInfo.innerHTML = ` 
            <p>Looks like this post doesn't have a comments section! Comments are based off bluesky posts from my account, tagged to link to here. 
            Either I've decided that this post shouldn't have comments, or I've forgotten. You can remind me, or ask me any questions / make any comments in my dms though.
            Hit me up on <a href="https://bsky.app/profile/epsirho.com">Bluesky</a> or <a href="https://furry.engineer/@epsi">Mastodon</a>, or if need be my <a href="mailto:epsirho@gmail.com">Email</a></p>
        `;

        commentsContainer.appendChild(noCommentsInfo);
      }
      else{
        const hdr = document.getElementById('comments-header');
        const addComment = document.createElement("a");

        addComment.href = ConnectPostUrl;
        addComment.innerHTML = ` 
            <p title="Add a comment with bluesky">&#x1F5E8</p>
        `;

        hdr.appendChild(addComment);
      }

      loadingBarContainer.style.display = "none";
    })
    .catch((error) => {
      console.error("Error:", error);
      loadingBarContainer.style.display = "none";
    });
});
