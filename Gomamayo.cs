using System;
using MeCab;

namespace GomamayoNET{
    class Gomamayo{

        public int GomamayoThreshold = 5;
        public bool IsGomamayo(string text){
            MeCabParam mecabParam = new MeCabParam();
            MeCabTagger tagger = MeCabTagger.Create(mecabParam);
            string beforeSurface = "";
            bool beforeIsNoun = false;

            foreach (var node in tagger.ParseToNodes(text)){
                if (node.CharType > 0){
                    var features = node.Feature.Split(',');
                    var displayFeatures = string.Join(", ", features);
                    Console.WriteLine($"{node.Surface}\t{displayFeatures}\nBeforeSurface: {beforeSurface}");

                    //記号の場合は飛ばす
                    if (features[0] == "記号"){
                        continue;
                    }

                    // 前のノードが名詞で、現在のノードも名詞であり、その音が前のノードと同じである場合、ゴママヨと判定
                    if (beforeIsNoun && features[0] == "名詞" && features[7][0] == beforeSurface[beforeSurface.Length - 1]){
                        return true;
                    }

                    // 現在のノードが名詞である場合、フラグを設定
                    if (features[0] == "名詞"){
                        beforeIsNoun = true;
                        beforeSurface = features[7];
                    }
                    else{
                        beforeIsNoun = false;
                    }
                }
            }

            Console.WriteLine($"{text} はごまマヨじゃない");
            return false;
        }

        public bool IsHigherGomamayo(String text){
            if(GomamayoThreshold < 1){
                throw new Exception("GomamayoThreshold must be greater than 0.");
            }
            MeCabParam mecabParam = new MeCabParam();
            MeCabTagger tagger = MeCabTagger.Create(mecabParam);
            string beforeSurface = "";
            bool beforeIsNoun = false;

            foreach (var node in tagger.ParseToNodes(text)){
                if (node.CharType > 0){
                    var features = node.Feature.Split(',');
                    var displayFeatures = string.Join(", ", features);
                    Console.WriteLine($"{node.Surface}\t{displayFeatures}\nBeforeSurface: {beforeSurface}");

                    //記号の場合は飛ばす
                    if (features[0] == "記号"){
                        continue;
                    }

                    // 前のノードが名詞で、現在のノードも名詞であり、その音が前のノードと同じである場合、ゴママヨと判定
                    for(int i = 0; i < GomamayoThreshold; i++){
                        if(features[7].Length <= i || beforeSurface.Length <= i){
                            break;
                        }
                        if (beforeIsNoun && features[0] == "名詞" && features[7][i] == beforeSurface[beforeSurface.Length - i-1]&&i!=0){
                            return true;
                        }
                    }

                    // 現在のノードが名詞である場合、フラグを設定
                    if (features[0] == "名詞"){
                        beforeIsNoun = true;
                        beforeSurface = features[7];
                    }
                    else{
                        beforeIsNoun = false;
                    }
                }
            }

            Console.WriteLine($"{text} は高次ゴママヨじゃない");
            return false;
        }
    }
}