import  Card  from "react-bootstrap/Card";
import { Link } from "react-router-dom";
import { isEmplyOrSpaces } from "../../../Utils/Utils";
import TagList from "../tags/TagList";

const PostList = ({postItem})=>{
    let imageUrl = isEmplyOrSpaces(postItem.imageUrl)
    ?process.env.PUBLIC_URL + '/images/image_1.jpg'
    :`https://localhost:7278/${postItem.imageUrl}`;
 
    return (
        <article className="blog-entry mb-4">
            <Card>
                <div className="row g-0">
                    <div className="col-md-4">
                        <Card.Img variant="top" src={imageUrl} alt={postItem.Title} />

                    </div>

                    <div className="col-md-8">
                        <Card.Body>
                            <Card.Title>
                                <Link to={`/blog/post/${postItem.urlSlug}`}>
                                {postItem.title}
                                </Link>
                            </Card.Title>
                            <Card.Text>
                                <small className="text-muted">Tác giả:</small>
                                <span className="text-primary m-1">
                                   <Link to={`/blog/author/${postItem.author.urlSlug}`}>
                                   {postItem.author.fullName}
                                   </Link>
                                </span>
                                <small className="text-muted">Chủ đề</small>
                                <span className="text-primary m-1">
                                    
                                    <Link to={`/blog/category/${postItem.category.urlSlug}`}>
                                    {postItem.category.name}
                                   </Link>
                                </span>
                            </Card.Text>
                            <Card.Text>
                                {postItem.shortDescription}
                            </Card.Text>
                            <div className="tag-list">
                                <TagList tagList={postItem.tags} />
                            </div>
                            <div className="text-end">
                                <Link
                                to={`/post/${postItem.urlSlug}`}
                                    className="btn btn-primary"
                                    title={postItem.Title}>
                                        Xem chi tiết
                                    </Link>
                            </div>
                        </Card.Body>
                    </div>
                </div>
            </Card>
        </article>
    )
}
export default PostList