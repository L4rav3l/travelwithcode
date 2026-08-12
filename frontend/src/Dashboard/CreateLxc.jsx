import React, { useEffect, useState } from "react";
import axios from "axios";

function CreateLxc()
{

    const [haveLXC, setHaveLXC] = useState(true);
    const [repos, setRepos] = useState([]);
    const [selectedRepo, setSelected] = useState();
    const [lxcId, setLxcId] = useState();

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    useEffect(() => {
        const checkLxc = async () => {
            try
            {
                const response = await axios.get("/api/proxmox/lxc", {headers: {Authorization: `Bearer ${token}`}})
                
                if(response.data === 0)
                {
                    setHaveLXC(false);
                } else {
                    setHaveLXC(true);
                    setLxcId(response.data);
                }
            }
            catch
            {

            }
        }

        const githubRepos = async () => {
            try
            {
                const response = await axios.get("/api/github/repos", {headers: {Authorization: `Bearer ${token}`}});
                setRepos(response.data);
                console.log(response.data);
            }
            catch(ex)
            {
                alert("GITHUB TOKEN EXPIRED OR INVALID.");
            }
        }

        checkLxc();
        githubRepos();
    }, [])

    const deleteLxc = async () => {
        try
        {
            const response = await axios.post("api/proxmox/delete_lxc", {headers: {Authorization: `Bearer ${token}`}});
            setHaveLXC(true);
        }
        catch
        {

        }
    }

    const createLxc = async () => {
        try
        {
            const response = await axios.post("api/proxmox/create_lxc", {githubRepo: selectedRepo}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {

        }
    }

    return (
        <>
            <div class="h-screen flex flex-col justify-center items-center">
                <div class="flex flex-col gap-1 bg-white/10 items-center justify-center p-4 rounded-lg">
                    {haveLXC === true && (<><span class="text-white">https://{lxcId}.{window.location.hostname}</span><span class="text-white">Username: lxc</span><span class="text-white">Password: }fTT4cT08xw,</span><button class="bg-red-600 p-2 rounded-lg text-white" onClick={() => {deleteLxc()}}>Destroy container</button> <span class="text-white">We won't commit the last version of your file!</span> </>)}
                    {haveLXC === false &&(
                        <div class="flex flex-col w-64 gap-2">
                            <span class="text-white text-center">
                                Repository
                            </span>
                            <select class="bg-white/10 rounded-lg text-white" value={selectedRepo} onChange={(e) => setSelected(e.target.value)}>
                                {repos.map((repo, index) => (
                                    <>
                                        <option value={repo} class="bg-gray-800 text-white">{repo}</option>
                                    </>
                                ))}
                            </select>
                            <button class="bg-white/10 rounded-lg p-1 text-white" onClick={() => {createLxc()}}>Create Container</button>
                        </div>
                    )}

                </div>
            </div>
        </>
    );
}

export default CreateLxc;