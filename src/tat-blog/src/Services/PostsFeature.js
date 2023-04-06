import axios from "axios";
export async function getPostsFeature(limit=3){
    try{
        const response= await axios.get(`https://localhost:7278/api/posts/featured/${limit}`);
        const data = response.data;
        if(data.isSuccess){
            return data.result;
        }
        else
            return null;
    }catch(error){
            console.log('Error', error.message);
            return null;
    }
}