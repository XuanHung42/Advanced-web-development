import { useQuery } from "../Utils/Utils";

const BadResquest=()=> {
    let query = useQuery(),
    redirectTo = query.get('redirectTo')?? '/';
    return(
        <>
        ...400
        </>
    )
}
export default BadResquest;