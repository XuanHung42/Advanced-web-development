import React, { useEffect, useState } from 'react'
import Pager from '../../Pager';
import PostList from './PostItem';
import { getPosts } from '../../../Services/BlogRepository';


const PostSearch = ({postQuery}) => {
    const {
        querySearch, params
    } = postQuery;

    const [pageNumber, setPageNumber] = useState(1);
    const [postsList, setPostsList] = useState({
        items: [],
        metadata: {}
    });

    useEffect(() => {
        loadBlogPosts();

        async function loadBlogPosts() {
            const parameters = new URLSearchParams({
              pageNumber: Object.fromEntries(querySearch || '').length > 0 ? 1 : pageNumber || 1,
              pageSize: 3,
              ...Object.fromEntries(querySearch || ''),
              ...params,
            });
            getPosts(parameters).then((data)=>{
              if(data){
                setPostsList(data)
              }
              else{
                setPostsList([])
              }
            })
        }
    }, [pageNumber, params, querySearch]);

    function updatePageNumber(inc) {
        setPageNumber((currentVal) => currentVal + inc);
    }

    return (
        <div className="posts-wrapper">
            {postsList.items.map((item )=>(
              <PostList postItem={item}/>

             
            ))}
            <Pager 
                metadata={postsList.metadata}
                onPageChange={updatePageNumber}/>
               
        </div>
    )
}

export default PostSearch;