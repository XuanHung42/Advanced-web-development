import { useState , useEffect} from "react";
import { useParams } from "react-router-dom"
import { isEmplyOrSpaces } from "../Utils/Utils";
import { getPostBySlug } from "../Services/BlogRepository";
import { Link } from "react-router-dom";


const PostDetail=()=>{
    const params = useParams();
    const [post, setPosts]= useState('');
    const {slug }= params;
    let imageUrl = !post|| isEmplyOrSpaces(post.imageUrl)
        ?process.env.PUBLIC_URL + "/images/image_1.jpg"
        : `https://localhost:7278/${post.imageUrl}`;

        useEffect(()=> {
            getPostBySlug(slug).then((data)=>{
                if(data){
                    setPosts(data);

                }
                else{
                    setPosts([]);
                }
            });
        },[slug]);
        if(post){
            return(
                <div className="mt-3">
                <h1 className="text-success">{post.title}</h1>
                <p className="card-text">
                  <span className="row">
                    <small className="text-danger col-4">
                      Tác giả:
                     {
                      
                        <Link className="text-decoration-none">
                        {post.author.fullName}
                      </Link>
                      
                     }
                    </small>
        
                    <small className="text-danger col-4">
                      Ngày đăng:
                      <span className="text-success">{post.postedDate}</span>
                    </small>
        
                    <small className="text-danger col-4">
                      Lượt xem:
                      <span className="text-success">{post.viewCount}</span>
                    </small>
                  </span>
                </p>
        
                <p className="card-text">
                  <span>
                    <small className="text-danger">Chủ đề:</small>
                    <Link className="text-decoration-none">{post.category.name}</Link>
                  </span>
                </p>
        
                <p className="card-text">
                  <span>
                    <small className="text-danger">Tags:</small>
                    {post.tags.map((tag, index) => (
                      <Link to={`/blog/tag/${tag.name}`} key={index} className="btn btn-outline-primary btn-sm me-2 mb-2">
                        {tag.name}
                      </Link>
                    ))}
                  </span>
                </p>
        
                <div className="text-danger mt-3 mb-3">
                  Short Description
                  <span className="text-dark">{post.meta}</span>
                </div>
        
                <img src={imageUrl} alt={post.title} width={"100%"} />
        
                <div className="text-danger mt-3">
                  Description
                  <span className="text-dark">{post.description}</span>
                </div>
              </div>
            );
        }
        else{
            return <></>
        }
}
export default PostDetail