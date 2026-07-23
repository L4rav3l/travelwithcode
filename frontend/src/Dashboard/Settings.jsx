import axios from "axios";
import React, { useEffect, useState } from "react";

function Settings()
{

    const [username, setUsername] = useState("");
    const [usernamePassword, setUsernamePassword] = useState("");

    const [newPassword, setNewPassword] = useState("");
    const [oldPassword, setOldPassword] = useState("");

    const [setupScript, setSetupScript] = useState("");

    const [githubtoken, setGithubToken] = useState("");

    const [loading, setLoading] = useState(false);

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    const handleUsername = async () => {
        try
        {
            setLoading(true);
            const response = await axios.post("/api/profile/change_username", {Username: username, Password: usernamePassword}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {
            alert("The username is reserved.");
        }
        finally
        {
            setLoading(false);
        }
    }

    const handlePassword = async () => {
        try
        {
            setLoading(true);
            const response = await axios.post("/api/profile/change_password", {OldPassword: oldPassword, NewPassword: newPassword}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {
            alert("ERROR");
        }
        finally
        {
            setLoading(false);
        }
    }

    const handleScript = async () => {
        try
        {
            setLoading(true);
            const response = await axios.post("/api/profile/change_script", {Script: setupScript}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {
            alert("ERROR");
        }
        finally
        {
            setLoading(false);
        }
    }

    const handleToken = async () => {
        try
        {
            setLoading(true);
            const response = await axios.post("/api/github/set_token", {githubToken: token}, {headers: {Authorization: `Bearer ${token}`}})
        }
        catch
        {
            alert("ERROR");
        }
        finally
        {
            setLoading(false);
        }

    }

    useEffect(() => {
        const handleData = async () => {

            try
            {
                const response = await axios.get("/api/profile/script", {headers: {Authorization: `Bearer ${token}`}});
                setSetupScript(response.data);
            }
            catch
            {

            }

        };

        handleData();
    }, []);

    return (
        <>
            <div class="h-screen flex flex-col justify-center items-center">
                <div class="flex flex-row bg-white/10 rounded-lg justify-center items-center p-2 gap-8 grid grid-cols-2">
                    
                    <div class="flex flex-col bg-white/10 p-2 rounded-lg gap-2 w-96">
                        <span class="text-white">Change Username</span>

                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">New username</span>
                            <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => {setUsername(e.target.value)}} value={username}/>
                        </div>

                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">Password</span>
                            <input type="password" class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => {setUsernamePassword(e.target.value)}} value={usernamePassword}/>
                        </div>

                        <button class="bg-white/10 p-1 text-white rounded-lg" disabled={loading} onClick={() => {handleUsername()}}>Change Username</button>
                    </div>

                    <div class="flex flex-col bg-white/10 p-2 rounded-lg gap-2">
                        <span class="text-white">Change password</span>

                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">New password</span>
                            <input class="bg-white/10 p-1 rounded-lg text-white" type="password" onChange={(e) => {setNewPassword(e.target.value)}} value={newPassword} />
                        </div>

                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">Old password</span>
                            <input class="bg-white/10 p-1 rounded-lg text-white" type="password" onChange={(e) => {setOldPassword(e.target.value)}} value={oldPassword}/>
                        </div>

                        <button class="bg-white/10 p-1 text-white rounded-lg" disabled={loading} onClick={() => {handlePassword()}}>Change password</button>

                    </div>

                    <div class="flex flex-col bg-white/10 p-2 rounded-lg gap-2">
                    
                        <span class="text-white">Change your setup script</span>
                        
                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">Bash Script</span>
                            <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => {setSetupScript(e.target.value)}} value={setupScript}/>
                        </div>

                        <button class="bg-white/10 p-1 text-white rounded-lg" disabled={loading} onClick={() => {handleScript()}}>Change setup script</button>
                    </div>

                    <div class="flex flex-col bg-white/10 p-2 rounded-lg gap-2">
                    
                        <span class="text-white">Change your github token</span>

                        <div class="flex flex-col gap-1">
                            <span class="text-white text-sm">Github Token</span>
                            <input type="password" class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => {setGithubToken(e.target.value)}} value={githubtoken}/>
                        </div>

                        <button class="bg-white/10 p-1 text-white rounded-lg" disabled={loading} onClick={() => {handleToken()}}>Change Github Token</button>

                    </div>
               
                </div>
            </div>
        </>
    );
}
export default Settings;