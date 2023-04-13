import React from "react";
import { useLocation, useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import PostList from "../Components/Blog/posts/PostItem";
import Pager from "../Components/Pager";
import { getPostByTagSlug } from "../Services/BlogRepository";


export default function PostsByTag() {
  const params = useParams();
  const [posts, setPosts] = useState([]);
  const [metadata, setMetadata] = useState({});

  function useQuery() {
    const { search } = useLocation();
    return React.useMemo(() => new URLSearchParams(search), [search]);
  }

  const query = useQuery(),
    pageNumber = query.get("p") ?? 1,
    pageSize = query.get("ps") ?? 3;

  useEffect(() => {
    loadPostsByTagSlug();
    async function loadPostsByTagSlug() {
      const data = await getPostByTagSlug(params.slug, pageSize, pageNumber);
      if (data) {
        setPosts(data.items);
        setMetadata(data.metadata);
      } else setPosts([]);
    }
  }, [params, pageSize, pageNumber]);

  return (
    <div className="p-4">
      {posts.map((item) => {
        return <PostList key={item.id} postItem={item} />;
      })}
      <Pager postQuery="" metadata={metadata} />
    </div>
  );
}