import { useState, useEffect } from "react";
import  ListGroup  from "react-bootstrap/ListGroup";
import {Link } from "react-router-dom";
import { getTagsCloud } from "../Services/TagCloud";


const TagsCloudWidget = ()=>{
    const[TagsCloud, setTagsCloud]= useState([]);
    useEffect(()=> {
        getTagsCloud().then(data=>{
            if(data){
                setTagsCloud(data)
            }
            else
            {
                setTagsCloud([])
            }
        });
    }, [])
    return(
        <div className="mb-4">
        <h3 className="text-success mb2">
            Tags Cloud
        </h3>
        {TagsCloud.length >0 &&
        <ListGroup>
            {TagsCloud.map((item, index)=> {
                return(
                    <ListGroup.Item key={index}>
                        <Link to={`/blog/tag?slug=${item.urlSlug}`}
                        title={item.description}
                        key={index}>
                            {item.name}
                            <span>&nbsp; ({item.postCount}) </span>
                        </Link>
                    </ListGroup.Item>
                )
            })}
            </ListGroup>}
    </div>
    )
}
export default TagsCloudWidget