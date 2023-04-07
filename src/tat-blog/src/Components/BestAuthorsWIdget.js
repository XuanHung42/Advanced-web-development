import { useState, useEffect } from "react";
import  ListGroup  from "react-bootstrap/ListGroup";
import {Link } from "react-router-dom";
import { getBestAuthors } from "../Services/BestAuthors";
const BestAuthorsWidget = ()=>{
    const[BestAuthors, setBestAuthors]= useState([]);
    useEffect(()=> {
        getBestAuthors().then(data=>{
            if(data){
                setBestAuthors(data)
            }
            else
            {
                setBestAuthors([])
            }
        });
    }, [])
return(
    <div className="mb-4">
        <h3 className="text-success mb2">
            Tác giả nổi bật
        </h3>
        {BestAuthors.length >0 &&
        <ListGroup>
            {BestAuthors.map((item, index)=> {
                return(
                    <ListGroup.Item key={index}>
                        <Link to={`/blog/author/best/?slug=${item.urlSlug}`}
                        title={item.fullName}
                        key={index}>
                            {item.fullName}
                            <span>&nbsp; ({item.postCount}) </span>
                        </Link>
                    </ListGroup.Item>
                )
            })}
            </ListGroup>}
    </div>
)
}

export default BestAuthorsWidget