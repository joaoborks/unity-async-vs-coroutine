![](https://img.shields.io/github/license/JoaoBorks/unity-async-vs-coroutine?style=flat)
# Unity Async vs Coroutine
A quick performance benchmark test on Unity, comparing async usage with Unity Coroutines.

## How to test
Open the project on the Unity Editor, and select `Window>General>Test Runner`. On the Test Runner Window, select `Play Mode` and then `Run All`. This will run all tests in a runtime simulation.
The tests produce a json output on the `Output` folder. On this same folder there is an Office Excel file which displays the data in a comparison chart, that you can easily update to match your tests.

You can change test parameters, such as simulation count and initial threshold, on the `BenchmarkSettings` asset under `Assets/Resources`.

## Contributing

Feel free to contribute to these tests if you wish. My only purpose with this repository is to answer questions with solid testing data. If you think the tests can be improved or are generating unreliable data, please add your own contribuition as this will help the entire community.
