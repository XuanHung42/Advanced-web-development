import { useState, useEffect } from "react";
import  ListGroup  from "react-bootstrap/ListGroup";
import {Link } from "react-router-dom";
import { getPostsFeature } from "../Services/PostsFeature";
const PostsFeatureWidget=()=>{
    const[PostsFeature, setPostsFeature]= useState([]);
    useEffect(()=> {
        getPostsFeature().then(data=> {
            if(data){
                setPostsFeature(data)
            }
            else{
                setPostsFeature([])
            }
        });
    },[])
    return(
        <div className="mb-4">
            <h3 className="text-success mb2">
                Các bài viết nổi bật
            </h3>
            {PostsFeature.length >0 &&
            <ListGroup>
                {PostsFeature.map((item, index)=> {
                    return(
                        <ListGroup.Item key={index}>
                            <Link to={`/blog/category?slug=${item.urlSlug}`}
                            title={item.description}
                            key={index}>
                                {item.title}
                                <span>&nbsp; ({item.viewCount}) </span>
                            </Link>
                        </ListGroup.Item>
                    )
                })}
                </ListGroup>}
        </div>
    )
}
export default PostsFeatureWidget