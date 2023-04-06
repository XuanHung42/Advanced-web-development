import { useState, useEffect } from "react";
import  ListGroup  from "react-bootstrap/ListGroup";
import {Link } from "react-router-dom";
import { getPostsRadom } from "../Services/PostsRadom";
const PostRadomWidget=()=>{
    const[PostsRadom, setPostsRadom]= useState([]);
    useEffect(()=> {
        getPostsRadom().then(data=> {
            if(data){
                setPostsRadom(data)
            }
            else{
                setPostsRadom([])
            }
        });
    },[])
    return(
        <div className="mb-4">
            <h3 className="text-success mb2">
                Các bài viết ngẫu nhiên
            </h3>
            {PostsRadom.length >0 &&
            <ListGroup>
                {PostsRadom.map((item, index)=> {
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
export default PostRadomWidget