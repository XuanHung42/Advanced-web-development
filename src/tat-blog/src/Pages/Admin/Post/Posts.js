import React, { useEffect, useState } from "react";
import {getPosts} from '../../../Services/BlogRepository'

const Posts=()=>{
    const[postsList, setPostsList]= useState([]);
    const[isVisibleLoading, setIsVisibleLoading] = useState([]);
    let k='', p=1, ps =3;
    useEffect(()=> {
        document.title = 'Danh sách bài viết';
        getPosts(k,ps,p).then(data =>
            {
                if(data)
                    setPostsList(data.items);
                else
                    setPostsList([]);
                    setIsVisibleLoading(false);
            })
    },[k,p,ps]);
    return(
        <>
        <h1> Danh sách bài viết</h1>
        </>
    )
}

export default Posts