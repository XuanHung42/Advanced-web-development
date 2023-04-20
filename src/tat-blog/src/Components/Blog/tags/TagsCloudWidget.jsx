import { useState, useEffect } from "react";
import {Link } from "react-router-dom";
import { getTagsCloud } from "../../../Services/TagRepository";


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
       
            {TagsCloud.map((item, index)=> {
                return(
                    // <ListGroup.Item key={index}>
                    //     <Link to={`/blog/tag?slug=${item.urlSlug}`}
                    //     title={item.description}
                    //     key={index}>
                    //         {item.name}
                    //         <span>&nbsp; ({item.postCount}) </span>
                    //     </Link>
                    // </ListGroup.Item>

                    <Link key={index} className="btn btn-outline-primary btn-sm me-2 mb-2"
                    to={`/blog/tag/${item.urlSlug}`}
                    title="Xem chi tiết">
                        {item.name}
                        <span>&nbsp; ({item.postCount}) </span>
                    </Link>
                )
            })}
       
    </div>
    )
}
export default TagsCloudWidget