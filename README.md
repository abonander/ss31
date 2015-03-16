## ss31 [![Build Status](https://travis-ci.org/ss31/ss31.svg?branch=master)](https://travis-ci.org/ss31/ss31)
Remake of the cult classic Space Station 13, now with C# and [Monogame](http://www.monogame.net/), made cross platform by the [Mono Framework](http://www.mono-project.com/).

Social
-
* Reddit: [/r/SS31](http://www.reddit.com/r/SS31), discussions and official news forum.
* Waffle.io: [waffle.io](https://waffle.io/ss31/ss31), our agile development board, see who is working on what.
* Gitter: [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/ss31/ss31), come join the devs as they make boring chat about the work.
* AppVeyor: [Build Status](https://ci.appveyor.com/project/cybergeek94/ss31), see the status of our latest builds, and download to test for yourself.

Authors
-
* AlmostPete: Programmer
* cybergeek94: Programmer
* johnku1: Programmer
* Starrider814: 3D Modeler

Cloning
-

Once content begins to appear in the `ss31-content` repository, it will be necessary to pull in the submodule during the build process.

If your Git is up to date, you can grab all submodules by cloning with the `--recursive` flag:

    git clone --recursive https://github.com/ss31/ss31

To update your existing repository:

    git pull
    git submodule update --init

#####Updating the `ss31-content` Reference:
When the `ss31-content` repo is updated, the submodule reference in this repo will need to be updated to point to the new commit.

This can be done by running the following command (using latest Git):

    git submodule update --remote --merge

And then commit and push the changed files to this repository (pull request recommended).

License
-
All human-readable source code materials in this repository are licensed under the GPL, version 3.

All images, models, and rigging files in this repository are licensed under the Creative Commons Attribution-ShareAlike 3.0 United States license.

Warranty
-
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
