import axios from "axios";
import { useActionState, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaKey } from "react-icons/fa";
import { FaUser } from "react-icons/fa";

function Login()
{

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [remember, setRemember] = useState(false);
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    useEffect(() => {

        if(localStorage.getItem("token") || sessionStorage.getItem("token"))
        {
            navigate("/dashboard");
        }

    }, [])


    const handleLogin = async () => {
        try
        {
            setLoading(true);
            const response = await axios.post("/api/auth/login", {Username: username, Password: password});

            if(remember === true)
            {
                localStorage.setItem("token", response.data.token);
            } else {
                sessionStorage.setItem("token", response.data.token);
            }

            navigate("/dashboard");
        }
        catch
        {

        }
        finally
        {
            setLoading(false);
        }
    }


    return (
        <>
            <div class="h-screen flex justify-center items-center">
                <div class="flex flex-col bg-white/10 backdrop-blur p-2 w-96 rounded-lg gap-2">
                    <span class="text-white text-semibold text-center text-2xl">Login</span>
                    
                    <div class="flex flex-col">
                        <div class="flex flex-row p-1 items-center gap-2">
                            <FaUser class="text-white" />
                            <span class="text-white">Username</span>
                        </div>
                            <input class="p-1 rounded-lg bg-white/10 backdrop-blur text-white" onChange={(e) => {setUsername(e.target.value)}} value={username}/>
                    </div>

                    <div class="flex flex-col">
                        <div class="flex flex-row items-center gap-2">
                            <FaKey class="text-white" />
                            <span class="text-white">Password</span>
                        </div>
                        <input type="password" class="p-1 rounded-lg bg-white/10 backdrop-blur text-white" onChange={(e) => setPassword(e.target.value)} value={password}/>
                    </div>

                    <div class="flex flex-row p-1 items-center gap-2">
                        <input type="checkbox" onChange={(e) => (setRemember(e.target.checked))} value={remember} class="w-4 h-4 rounded text-violet-600 focus:ring-violet-500" />
                        <span class="text-white select-none">Remember you?</span>
                    </div>

                    <button class="bg-white/10 p-2 rounded-lg text-white hover:[transform:scale(1.01)]" disabled={loading} onClick={() => {handleLogin()}}>Login</button>
                </div>
            </div>
        </>
    );

}

export default Login;